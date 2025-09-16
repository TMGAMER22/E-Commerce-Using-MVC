using Microsoft.AspNetCore.Mvc;
using Store.Models;
using Store.Repositories.Orders;
using Store.ViewModel.Order;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Store.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderRepository orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            this.orderRepository = orderRepository;
        }
        [HttpGet]
        public async Task<IActionResult> CheckoutPage(Guid? id)
        {
            Order order = null;

            // البحث باستخدام ID إذا كان موجوداً
            if (id.HasValue)
            {
                order = await orderRepository.GetOrderByIdAsync(id.Value);
            }
            else
            {
                // البحث عن آخر طلب غير مكتمل للمستخدم
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userId))
                {
                    order = await orderRepository.GetLatestPendingOrderAsync(userId);

                    // إذا لم يوجد طلب معلق، ننشئ واحد جديد
                    if (order == null)
                    {
                        return RedirectToAction(nameof(MyOrder));
                    }
                }
            }

            if (order == null)
                return NotFound();

            var viewModel = new OrderViewModel
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                Status = order.Status,
                TotalPrice = order.TotalPrice,
                Notes = order.Notes,
                Items = order.Items.Select(i => new OrderItemViewModel
                {
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> MyOrder()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var order = await orderRepository.GetOrCreatePendingOrderAsync(userId);

            return RedirectToAction(nameof(CheckoutPage), new { id = order.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var order = await orderRepository.FillOrderFromCartAsync(userId);

            return RedirectToAction(nameof(CheckoutPage), new { id = order.Id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveOrderNotes(Guid id, string? notes)
        {
            var order = await orderRepository.GetOrderByIdAsync(id);
            if (order == null) return NotFound();

            await orderRepository.UpdateOrderNotesAsync(id, notes);
            return RedirectToAction(nameof(CheckoutPage), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmOrder(Guid id)
        {
            var order = await orderRepository.GetOrderByIdAsync(id);
            if (order == null) return NotFound();
            // Here you might change status or trigger further processing if needed
            return RedirectToAction(nameof(CheckoutPage), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            await orderRepository.DeleteOrderFromUserAsync(userId);
            return RedirectToAction("ShowAllProducts", "Product");
        }


    }
}
