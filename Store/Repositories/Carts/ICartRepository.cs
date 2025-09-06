using Store.Models;

namespace Store.Repositories.Carts
{
    public interface ICartRepository
    {
        Task AddToCart(string userId, Guid productId, int quantity);
        Task ClearCart(string userId);
        Task<CartItem?> GetItemById(Guid id);
        Task<Cart?> GetCartByUserId(string userId);
        Task<bool> IncreaseCartItem(string userId, Guid cartItemId);
        Task<bool> DecreaseCartItem(string userId, Guid cartItemId);

    }
}
