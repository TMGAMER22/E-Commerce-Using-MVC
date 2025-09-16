using Store.Models;

namespace Store.Repositories.Orders
{
    public interface IOrderRepository
    {
        Task<Order?> GetLatestPendingOrderAsync(string userId);
        Task<Order> GetOrCreatePendingOrderAsync(string userId);
        Task<Order> FillOrderFromCartAsync(string userId);
        Task<Order?> GetOrderByIdAsync(Guid orderId);
        Task<Order> GetOrderByUserIdAsync(string userId);
        Task<bool> DeleteOrderFromUserAsync(string userId);
        Task<bool> UpdateOrderNotesAsync(Guid orderId, string? notes);
    }

}
