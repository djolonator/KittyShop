using KittyShop.Interfaces.IServices;
using KittyShop.Models;
using KittyShop.Services.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KittyShop.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICustomerService _userService;
        private readonly IAdminService _adminService;
        private readonly IHomeService _homeService;

        public HomeController(ILogger<HomeController> logger, ICustomerService kittyShopService, 
            IAdminService adminService, IHomeService homeService)
        {
            _logger = logger;
            _userService = kittyShopService;
            _adminService = adminService;
            _homeService = homeService;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel user)
        {
            if (ModelState.IsValid)
            {
                var result = await _homeService.Login(user);

                if (result.message == string.Empty)
                {
                    //var claims = new List<Claim>{new Claim("User", result.user.Type.ToString()),
                    //        new Claim("Admin", result.user.UserName),
                    //    };

                    var claims = new List<Claim>{
                        new Claim(ClaimTypes.Role, result.user.Type.ToString()),
                        new Claim(ClaimTypes.Name, result.user.UserName),
                        new Claim(ClaimTypes.SerialNumber, result.user.UserId.ToString())};
                    
                    var claimsIdentity = new ClaimsIdentity(
                    claims, "Login");

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity));



                    if (result.user.Type.ToString() == "Admin")
                        return RedirectToAction("Index", "Admin");
                    else
                        return RedirectToAction("Index", "Shop");
                }
                else
                {
                    ViewData["message"] = result.message;
                }
            }

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterModel user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _homeService.RegisterUser(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"User could not be created", ex);
            }

            return RedirectToAction("Fail");
        }

        public async Task<IActionResult> EditProfile()
        {
            var identity = (ClaimsIdentity)User.Identity!;
            var userId = int.Parse(identity.FindFirst(ClaimTypes.SerialNumber)!.Value);
            var model = await _homeService.GetUserAsync(userId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileModel user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var identity = (ClaimsIdentity)User.Identity!;
                    var userId = int.Parse(identity.FindFirst(ClaimTypes.SerialNumber)!.Value);
                    var rt = await _homeService.EditProfile(user, userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"User could not be edited", ex);
            }

            return RedirectToAction("Fail");
        }

        public async Task<IActionResult> ShopItemList(string furrColor, string eyesColor, 
            string description, string race, int? pageNumber)
        {
            int pageSize = 3;
            var products = await _homeService.GetProductsAsync(furrColor, 
                eyesColor, description, race, pageNumber, pageSize);
            
            return View(products);
        }
    }
}