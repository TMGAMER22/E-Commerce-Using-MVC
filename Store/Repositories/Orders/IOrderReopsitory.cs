using Store.Models;

namespace Store.Repositories.Orders
{
    public interface IOrderRepository
    {
        Task<Order> FillOrderFromCartAsync(string userId);
        Task<Order?> GetOrderByIdAsync(Guid orderId);
        Task<Order> GetOrderByUserIdAsync(string userId);
        Task<bool> DeleteOrderFromUserAsync(string userId);
    }

}
