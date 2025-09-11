using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Store.Models;

namespace Store.Repositories.Orders
{
    public class OrderRepository : IOrderRepository
    {
        private readonly MyContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public OrderRepository(MyContext _context , UserManager<ApplicationUser> userManager)
        {
            context = _context;
            this.userManager = userManager;
        }


        public async Task<Order> FillOrderFromCartAsync(string userId)
        {
            var userCart = await context.carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (userCart == null || !userCart.Items.Any())
                throw new Exception("Cart is empty!");

            var existingOrder = await context.orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.UserId == userId && o.Status == "Pending");

            Order order;
            if (existingOrder != null)
            {
                order = existingOrder;
            }
            else
            {
                order = new Order
                {
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    Status = "Pending",
                    TotalPrice = 0,
                    Items = new List<OrderItems>()
                };

                await context.orders.AddAsync(order);
            }

            foreach (var cartItem in userCart.Items)
            {
                var orderItem = new OrderItems
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Product.Price
                };

                order.Items.Add(orderItem);
                order.TotalPrice += cartItem.Quantity * cartItem.Product.Price;
            }

            context.cartItems.RemoveRange(userCart.Items);
            await context.SaveChangesAsync();

            return order;
        }


        public async Task<Order?> GetOrderByIdAsync(Guid orderId)
        {
            return await context.orders
             .Include(o => o.Items)
             .ThenInclude(i => i.Product)
             .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<Order> GetOrderByUserIdAsync(string userId)
        {
            return await context.orders.Include(o => o.Items)
                .ThenInclude(o => o.Product)
                .FirstOrDefaultAsync(o => o.UserId == userId);
            
        }
        public async Task<bool> DeleteOrderFromUserAsync(string userId)
        {
            var order = await context.orders
        .Include(o => o.Items)
        .ThenInclude(i => i.Product)
        .FirstOrDefaultAsync(o => o.UserId == userId);

            if (order == null)
                return false;

            foreach(var item in order.Items)
            {
                item.Product.StockQuantity += item.Quantity;
            }

            context.orders.Remove(order);

            await context.SaveChangesAsync();
            return true;
        }


    }
}
