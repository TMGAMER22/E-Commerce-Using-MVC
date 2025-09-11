using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store.Models;
using Store.Repositories.Products;
using Store.Repositories.Carts;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Store.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductRepository productRepository;
        private readonly ICartRepository cartRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public CartController(
            IProductRepository productRepository,
            ICartRepository cartRepository,
            UserManager<ApplicationUser> userManager)
        {
            this.productRepository = productRepository;
            this.cartRepository = cartRepository;
            this.userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(Guid productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var product = await productRepository.GetById(productId);
            if (product == null) return NotFound();

            await cartRepository.AddToCart(userId, productId, 1);

            if (product.StockQuantity > 0)
            {
                product.StockQuantity--;
                productRepository.Update(product);
                await productRepository.Save();
            }
            else
            {
                TempData["Error"] = "Sorry, this product is out of stock!";
            }

            return RedirectToAction("ShowAllProducts", "Product");
        }
        public async Task<IActionResult> ShowCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await cartRepository.GetCartByUserId(userId);
            return View(cart);
        }
        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await cartRepository.ClearCart(userId);
            return RedirectToAction("ShowCart");
        }
        [HttpPost]
        public async Task<IActionResult> IncreaseItem(Guid CartItemId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            bool success = await cartRepository.IncreaseCartItem(userId, CartItemId);

            if (!success)
            {
                ModelState.AddModelError("", "Sorry, this product is out of stock.");
            }

            var cart = await cartRepository.GetCartByUserId(userId);
            return View("ShowCart", cart);
        }
        [HttpPost]
        public async Task<IActionResult> DecreaseItem(Guid CartItemId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            bool success = await cartRepository.DecreaseCartItem(userId, CartItemId);

            if (!success)
            {
                ModelState.AddModelError("", "SomeThing Wrong!");
            }

            var cart = await cartRepository.GetCartByUserId(userId);
            return View("ShowCart", cart);
        }



    }
}
