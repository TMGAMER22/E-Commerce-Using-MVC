using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Store.Repositories.Categories;
using Store.ViewModel.Account;
using Store.Models;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Store.Repositories.Products;

namespace Store.Controllers
{
    public class AccountController : Controller
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IProductRepository productRepository;

        public AccountController(ICategoryRepository categoryRepository , UserManager<ApplicationUser> userManager 
            , SignInManager<ApplicationUser> signInManager , RoleManager<IdentityRole> roleManager , IProductRepository productRepository)
        {
            this.categoryRepository = categoryRepository;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.productRepository = productRepository;
        }


        private void PrepareRegisterViewData()
        {
            ViewData["Role"] = new List<SelectListItem>
            {
                new SelectListItem{Value="Customer",Text="Customer"},
                new SelectListItem{Value="Company",Text="Company"}
            };

            var categories = categoryRepository.GetAll();
            ViewData["Category"] = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
        }
        public IActionResult Register()
        {
            PrepareRegisterViewData();
            return View();
        }
        public async Task<IActionResult> SaveRegister(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                PrepareRegisterViewData();
                return View("Register", registerViewModel);
            }

            ApplicationUser AppUser = new ApplicationUser
            {
                UserName = registerViewModel.Name,
                CategoryId = registerViewModel.Role == "Company" ? registerViewModel.CategoryId : null
            };

            IdentityResult result = await userManager.CreateAsync(AppUser, registerViewModel.Password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(AppUser, registerViewModel.Role);
                await signInManager.SignInAsync(AppUser, isPersistent: true);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                PrepareRegisterViewData();
                return View("Register", registerViewModel);
            }
        }
        public IActionResult Login()
        {
            return View();
        }
        public async Task<IActionResult> SaveLogin(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser AppUser = await userManager.FindByNameAsync(loginViewModel.Name);
                if(AppUser == null)
                {
                    ModelState.AddModelError("", "Username Or Password Is Invalid");
                    return View("Login",loginViewModel);
                }
                else
                {
                    bool Found = await userManager.CheckPasswordAsync(AppUser, loginViewModel.Password);
                    if (Found == true)
                    {
                        await signInManager.SignInAsync(AppUser, loginViewModel.RememberMe);
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return View("Login", loginViewModel);
        }
        public async Task<IActionResult> SignOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
        public async Task<IActionResult> Delete()
        {
            var user = await userManager.GetUserAsync(User);

            if (user.CategoryId != null)
            {
                var userProducts = await productRepository.GetByUserId(user.Id); 
                if (userProducts.Any())
                {
                    ModelState.AddModelError("", "You Can't Delete This User Because You Have Products");
                    return View("Details", user);
                }
            }

            var result = await userManager.DeleteAsync(user);
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        public async Task<IActionResult> Details()
        {
            var user = await userManager.GetUserAsync(User); 
            var model = new Details
            {
                Name = user.UserName,
            };
            return View(model);
        }
        public async Task<IActionResult> SaveChanges(Details userDetails)
        {
            if (!ModelState.IsValid)
            {
                return View("Details", userDetails);
            }

            var user = await userManager.GetUserAsync(User);

            user.UserName = userDetails.Name;

            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = "Profile updated successfully!";
                return RedirectToAction("index","home");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View("Details", userDetails);
            }
        }
        public IActionResult EditPassword()
        {
            return View();
        }
        public async Task<IActionResult> SaveEditPassword(ChangePasswordViewModel UserViewModel)
        {
            if (ModelState.IsValid)
            {
                var OldPassword = UserViewModel.OldPassword;
                var NewPassword = UserViewModel.NewPassword;
                var user = await userManager.GetUserAsync(User);
                bool found = await userManager.CheckPasswordAsync(user, OldPassword);
                if (found)
                {
                    var result = await userManager.ChangePasswordAsync(user, OldPassword, NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Password change failed. Please try again.");
                        return View("EditPassword", UserViewModel);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Old password is incorrect.");
                    return View("EditPassword", UserViewModel);
                }
            }
            return View("EditPassword", UserViewModel);
        }


    }
}
