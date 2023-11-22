using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KittyShop.Controllers
{
    //[Authorize(Policy = "User")]
    [Authorize(Roles = "Regular")]
    public class ShopController : Controller
    {
        public ShopController() 
        {
        }

        public async Task<IActionResult> Index()
        {
            //var nameclaim = User.Claims.FirstOrDefault(c => c.Type == "Name");
            //var shopclaim = User.Claims.FirstOrDefault(c => c.Type == "Shop");

            var roleClaim = User.Claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault();
            return View();
        }
    }
}
