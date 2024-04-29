using KittyShop.Interfaces.IServices;
using KittyShop.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KittyShop.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeService _homeService;

        public HomeController(ILogger<HomeController> logger, IHomeService homeService)
        {
            _logger = logger;
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

                    if (result.userModel!.UserId != 0)
                    {
                        await SignInUser(MakeClaims(result.userModel));
                        
                        if (result.userModel.Type.ToString() == "Admin")
                            return RedirectToAction("Index", "Admin");
                        else
                            return RedirectToAction("Index", "Shop");
                    }
                    else SetMessageForUser(new MessageModel() { Message = result.message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Failed to login. Code exited with message {ex.Message} at {ex.StackTrace}");
                SetMessageForUser(new MessageModel() { Message = "Something went wrong!" });
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
        public async Task<IActionResult> Register(RegisterModel user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _homeService.RegisterUser(user);
                    SetMessageForUser(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Failed to register. Code exited with message {ex.Message} at {ex.StackTrace}");
                SetMessageForUser(new MessageModel() { Message = "Something went wrong!" });
            }

            return View(user);
        }

        public async Task<IActionResult> EditProfile()
        {
            try
            {
                var identity = (ClaimsIdentity)User.Identity!;
                var userId = int.Parse(identity.FindFirst(ClaimTypes.SerialNumber)!.Value);
                var user = await _homeService.GetUserAsync(userId);
                return View(user);
            }
            catch(Exception ex) 
            {
                _logger.LogCritical($"Failed to load edit profile page. Code exited with message {ex.Message} at {ex.StackTrace}");
                SetMessageForUser(new MessageModel() { Message = "Something went wrong, redirecting to index page" });
            }
            
            return RedirectToAction("Index", "Shop");
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
                    SetMessageForUser(result);
                    if (result.IsSuccess)
                        return View();
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Failed to edit profile. Code exited with message {ex.Message} at {ex.StackTrace}");
                SetMessageForUser(new MessageModel() { Message = "Something went wrong, redirecting to index page" });
            }

            return View(user);
        }

        public async Task<IActionResult> ShopItemList(string furrColor, string eyesColor, 
            string description, string race, int? pageNumber)
        {
            int pageSize = 6;
            var roleClaim = User.Claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault();

            PreserveSearchParametersThroughPagination(furrColor, eyesColor, description, race);

            try
            {
                var products = await _homeService.GetProductsAsync(furrColor,
                    eyesColor, description, race, pageNumber, pageSize);

                return View(products);
                
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Failed to load shop list page.Code exited with message {ex.Message} at {ex.StackTrace}", ex);
                SetMessageForUser(new MessageModel() { Message = "Something went wrong, redirecting to index page" });
            }
           
            return roleClaim!.Value == "Admin" ? RedirectToAction("Index", "Admin") : RedirectToAction("Index", "Shop");
        }

        private ClaimsIdentity MakeClaims(LoginModel userModel)
        {
            var claims = new List<Claim>{
                        new Claim(ClaimTypes.Role, userModel.Type.ToString()),
                        new Claim(ClaimTypes.Name, userModel.UserName),
                        new Claim(ClaimTypes.SerialNumber, userModel.UserId.ToString())};

            var claimsIdentity = new ClaimsIdentity(
            claims, "Login");

            return claimsIdentity;
        }

        private async Task SignInUser(ClaimsIdentity claimsIdentity)
        {
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));
        }

        private void PreserveSearchParametersThroughPagination(string furrColor, string eyesColor,
            string description, string race)
        {
            ViewData["furrColor"] = furrColor;
            ViewData["eyesColor"] = eyesColor;
            ViewData["description"] = description;
            ViewData["race"] = race;
        }

        private void SetMessageForUser(MessageModel result)
        {
            if (result.IsSuccess)
                TempData["successMessage"] = result.Message;
            else
                TempData["errorMessage"] = result.Message;
        }
    }
}