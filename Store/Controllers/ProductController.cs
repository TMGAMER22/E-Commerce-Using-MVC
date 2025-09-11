using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Store.Models;
using Store.Repositories.Products;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Store.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository productRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public ProductController(IProductRepository productRepository , UserManager<ApplicationUser> userManager)
        {
            this.productRepository = productRepository;
            this.userManager = userManager;
        }

        public IActionResult AddProduct()
        {
            return View();
        }
        public async Task<IActionResult> SaveAdd(Product ProductFromRequest , IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                string ImagePath = null;

                if(ImageFile!=null && ImageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(ImageFile.FileName);
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", fileName);
                    
                    using (var fileStream = new FileStream(uploadPath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }
                    ImagePath = "/Images/" + fileName;
                }


                var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var AppUser = userManager.FindByIdAsync(userid).Result;
                Product product = new Product
                {
                    Name = ProductFromRequest.Name,
                    Price = ProductFromRequest.Price,
                    StockQuantity = ProductFromRequest.StockQuantity,
                    CategoryId = AppUser.CategoryId.Value,
                    ImagePath = ImagePath,
                    UserId = AppUser.Id
                };
                productRepository.Add(product);
                productRepository.Save();
                return RedirectToAction("ShowUserProducts");
            }
            return View("AddProduct",  ProductFromRequest);
        }

        public async Task<IActionResult> ShowAllProducts(string searchQuery)
        {
            var products = await productRepository.GetAllWithCompany();
            if (!string.IsNullOrEmpty(searchQuery))
            {
                products = products
           .Where(p => p.Name.Contains(searchQuery))
           .ToList();
            }
            return View("ShowAllProducts", products); 
        }

        public async Task<IActionResult> ShowUserProducts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var products = await productRepository.GetByUserId(userId);
            return View("ShowUserProducts", products);
        }


        public async Task<IActionResult > EditProduct(Guid id)
        {
            var product = await productRepository.GetById(id);

            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEdit(Product productFromRequest)
        {
            if (!ModelState.IsValid)
                return View("EditProduct", productFromRequest);

            Product product = await productRepository.GetById(productFromRequest.Id);
            if (product == null)
                return NotFound(); 


            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
            if (product.UserId != currentUserId && !User.IsInRole("Company"))
                return Forbid();

            product.Name = productFromRequest.Name;
            product.Price = productFromRequest.Price;
            product.StockQuantity = productFromRequest.StockQuantity;
            product.CategoryId = productFromRequest.CategoryId; 
            product.ImagePath = productFromRequest.ImagePath;   

            productRepository.Update(product);
            productRepository.Save(); 

            return RedirectToAction("ShowUserProducts");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await productRepository.GetById(id);
            if (product == null)
                return NotFound();

            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (product.UserId != currentUserId && !User.IsInRole("Company"))
                return Forbid();

            productRepository.Delete(product.Id);
            productRepository.Save();
            return RedirectToAction("ShowUserProducts");
        }


    }
}
