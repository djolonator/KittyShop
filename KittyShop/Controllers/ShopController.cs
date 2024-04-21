using KittyShop.Interfaces.IServices;
using KittyShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KittyShop.Controllers
{
    //[Authorize(Policy = "User")]
    [Authorize(Roles = "Regular")]
    public class ShopController : Controller
    {
        private readonly IShopService _shopService;
        public ShopController(IShopService shopService) 
        {
            _shopService = shopService;
        }

        public async Task<IActionResult> Index()
        {
            //var nameclaim = User.Claims.FirstOrDefault(c => c.Type == "Name");
            //var shopclaim = User.Claims.FirstOrDefault(c => c.Type == "Shop");

            var roleClaim = User.Claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault();
            return View();
        }

        public async Task<IActionResult> ShoppingCart()
        {
            var identity = (ClaimsIdentity)User.Identity!;
            var userId = int.Parse(identity.FindFirst(ClaimTypes.SerialNumber)!.Value);
            var cart = await _shopService.GetShoppingCartForUser(userId);

            return View(cart);
        }

        [HttpPost]
        public async Task<JsonResult> AddToCart(int productId)
        {
            try
            {
                var identity = (ClaimsIdentity)User.Identity!;
                var userId = int.Parse(identity.FindFirst(ClaimTypes.SerialNumber)!.Value);

                var result = await _shopService.AddProductToCartAsync(userId, productId);

                //Prikazi result.message kao poruku
                
            }
            catch(Exception ex) 
            {

            }
            return Json(new { success = true });

        }

        //[HttpPost]
        //public async Task<JsonResult> UpdateCart(int cartId, int productId, int quantity)
        //{
        //    var result = new MessageModel();
        //    try
        //    {
        //        result = await _shopService.UpdateCartForUser(cartId, productId, quantity);
        //        //Prikazi result.message kao poruku

        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return Json(new { success = result.IsSuccess });

        //}

        [HttpPost]
        public async Task<IActionResult> UpdateCart(string quantity, string productId, string cartId)
        {
            var result = new MessageModel();
            try
            {
                result = await _shopService.UpdateCartForUser(int.Parse(cartId), int.Parse(productId), int.Parse(quantity));
                //Prikazi result.message kao poruku

            }
            catch (Exception ex)
            {

            }

            return RedirectToAction("ShoppingCart");
        }
    }
}
