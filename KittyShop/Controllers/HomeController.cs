using KittyShop.Data.Entities;
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
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _homeService.Login(user);
                    //result.message ide u alert
                    ViewData["Message"] = result.message;

                    if (result.user != null)
                    {
                        await SignInUser(MakeClaims(result.user));
                        
                        if (result.user.Type.ToString() == "Admin")
                            return RedirectToAction("Index", "Admin");
                        else
                            return RedirectToAction("Index", "Shop");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Shop item could not be added. Code exited with message {ex.Message} at {ex.StackTrace}");
            }
            return View(user);
        }

        private ClaimsIdentity MakeClaims(User user)
        {
            var claims = new List<Claim>{
                        new Claim(ClaimTypes.Role, user.Type.ToString()),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.SerialNumber, user.UserId.ToString())};

            var claimsIdentity = new ClaimsIdentity(
            claims, "Login");

            return claimsIdentity;
        }

        private async Task SignInUser(ClaimsIdentity claimsIdentity)
        {
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));
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
        public async Task<IActionResult> Register(RegisterModel user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _homeService.RegisterUser(user);
                    if (result.isRegisterSuccess)
                        return RedirectToAction("Login");
                    //result.message ide u notyf
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Shop item could not be added. Code exited with message {ex.Message} at {ex.StackTrace}");
            }

            //return RedirectToAction("Login");

            return View(user);
        }

        public async Task<IActionResult> EditProfile()
        {
            try
            {
                var identity = (ClaimsIdentity)User.Identity!;
                var userId = int.Parse(identity.FindFirst(ClaimTypes.SerialNumber)!.Value);
                var result = await _homeService.GetUserAsync(userId);
                if (!string.IsNullOrEmpty(result.user.UserName))
                    return View(result.user);
            }
            catch(Exception ex) 
            {
                _logger.LogCritical($"Shop item could not be added. Code exited with message {ex.Message} at {ex.StackTrace}");
            }
            //result.message ide u notyf
            return RedirectToAction("Login");
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
                    var result = await _homeService.EditProfile(user, userId);

                    if (!result.isEdited)
                        return View(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"User could not be edited", ex);
            }

            return RedirectToAction("Logout");
        }

        public async Task<IActionResult> ShopItemList(string furrColor, string eyesColor, 
            string description, string race, int? pageNumber)
        {
            int pageSize = 3;
            var roleClaim = User.Claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault();

            ViewData["furrColor"] = furrColor;
            ViewData["eyesColor"] = eyesColor;
            ViewData["description"] = description;
            ViewData["race"] = race;

            try
            {
                var products = await _homeService.GetProductsAsync(furrColor,
                    eyesColor, description, race, pageNumber, pageSize);

                if (products != null)
                {
                    return View(products);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"User could not be edited.Code exited with message {ex.Message} at {ex.StackTrace}", ex);
            }
            //var rt = MessagesConstants.SomethingWentWrong; u notyf
            return roleClaim!.Value == "Admin" ? RedirectToAction("Index", "Admin") : RedirectToAction("Index", "Shop");
        }
    }
}