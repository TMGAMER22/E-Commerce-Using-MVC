using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> CheckoutPage(Guid id)
        {
            var order = await orderRepository.GetOrderByIdAsync(id);

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = await orderRepository.FillOrderFromCartAsync(userId);

            return RedirectToAction(nameof(CheckoutPage), new { id = order.Id });
        }
        public async Task<IActionResult> CancelOrder()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await orderRepository.DeleteOrderFromUserAsync(userId);
            return RedirectToAction("ShowAllProducts", "Product");
        }


    }
}
