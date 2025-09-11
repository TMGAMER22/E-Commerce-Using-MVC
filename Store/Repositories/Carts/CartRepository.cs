using Microsoft.EntityFrameworkCore;
using Store.Models;

namespace Store.Repositories.Carts;

public class CartRepository : ICartRepository
{
    private readonly MyContext context;

    public CartRepository(MyContext _context)
    {
        context = _context;
    }

    public async Task AddToCart(string userId, Guid productId, int quantity)
    {
        var cart = await context.carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
        {
            cart = new Cart
            {
                Id = Guid.NewGuid(),
                UserId = userId
            };
            await context.carts.AddAsync(cart);
            await context.SaveChangesAsync();
        }

        var cartItem = await context.cartItems
            .FirstOrDefaultAsync(c => c.ProductId == productId && c.CartId == cart.Id);

        if (cartItem != null)
        {
            cartItem.Quantity += quantity;
            context.cartItems.Update(cartItem);
        }
        else
        {
            var newCartItem = new CartItem
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                Quantity = quantity,
                CartId = cart.Id
            };
            await context.cartItems.AddAsync(newCartItem);
        }

        await context.SaveChangesAsync();
    }

    public async Task ClearCart(string userId)
    {
        var cart = await context.carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product) 
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart != null && cart.Items != null)
        {
            foreach (var item in cart.Items)
            {
                if (item.Product != null)
                {
                    item.Product.StockQuantity += item.Quantity;
                    context.products.Update(item.Product);
                }
            }
            context.cartItems.RemoveRange(cart.Items);
            await context.SaveChangesAsync();
        }
    }


    public async Task<CartItem?> GetItemById(Guid id)
    {
        return await context.cartItems.FindAsync(id);
    }

    public async Task<Cart?> GetCartByUserId(string userId)
    {
        return await context.carts
            .Include(c => c.Items)
            .ThenInclude(c => c.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }


    public async Task<bool> IncreaseCartItem(string userId, Guid cartItemId)
    {
        var cart = await GetCartByUserId(userId);
        if (cart == null) return false;

        var cartItem = await context.cartItems
            .Include(c => c.Product)
            .FirstOrDefaultAsync(c => c.Id == cartItemId && c.CartId == cart.Id);

        if (cartItem == null || cartItem.Product == null)
            return false;

        if (cartItem.Product.StockQuantity <= 0)
            return false;

        cartItem.Quantity++;
        cartItem.Product.StockQuantity--;

        context.products.Update(cartItem.Product);
        context.cartItems.Update(cartItem);
        await context.SaveChangesAsync();

        return true; 
    }



    public async Task<bool> DecreaseCartItem(string userId, Guid cartItemId)
    {
        var cart = await GetCartByUserId(userId);
        if (cart == null) return false;

        var cartItem = await context.cartItems
            .Include(c => c.Product)
            .FirstOrDefaultAsync(c => c.Id == cartItemId && c.CartId == cart.Id);

        if (cartItem == null || cartItem.Product == null)
            return false;

        cartItem.Product.StockQuantity++;

        if (cartItem.Quantity > 1)
        {
            cartItem.Quantity--;
            context.cartItems.Update(cartItem);
        }
        else
        {
            context.cartItems.Remove(cartItem);
        }

        context.products.Update(cartItem.Product);
        await context.SaveChangesAsync();

        return true;
    }


}
