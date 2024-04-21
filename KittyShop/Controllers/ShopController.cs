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
        private readonly ILogger<ShopController> _logger;
        private readonly IShopService _shopService;
        public ShopController(IShopService shopService, ILogger<ShopController> logger)
        {
            _shopService = shopService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var roleClaim = User.Claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault();
            return View();
        }

        public async Task<IActionResult> ShoppingCart()
        {
            try
            {
                var identity = (ClaimsIdentity)User.Identity!;
                var userId = int.Parse(identity.FindFirst(ClaimTypes.SerialNumber)!.Value);
                var cart = await _shopService.GetShoppingCartForUser(userId);
                return View(cart);
            }

            catch (Exception ex)
            {
                _logger.LogCritical($"Shopping cart page failed to load. Code exited with message {ex.Message} at {ex.StackTrace}", ex);
            }

            SetMessageForUser(new MessageModel() { Message = "Something went wrong, redirecting to index page"});
            return RedirectToAction("Index", "Shop");
            
        }

        [HttpPost]
        public async Task<JsonResult> AddToCart(int productId)
        {
            var result = new MessageModel();
            try
            {
                var identity = (ClaimsIdentity)User.Identity!;
                var userId = int.Parse(identity.FindFirst(ClaimTypes.SerialNumber)!.Value);
                result = await _shopService.AddProductToUserCartAsync(userId, productId);
            }
            catch(Exception ex) 
            {
                _logger.LogCritical($"Failed to add item to cart. Code exited with message {ex.Message} at {ex.StackTrace}", ex);
            }

            return Json(new { success = result.IsSuccess, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCart(string quantity, string productId, string cartId)
        {
            var result = new MessageModel();
            try
            {
                result = await _shopService.UpdateCartForUser(int.Parse(cartId), int.Parse(productId), int.Parse(quantity));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Failed to update item in cart. Code exited with message {ex.Message} at {ex.StackTrace}", ex);
                result.Message = "Something went wrong, update item failed.";
            }

            SetMessageForUser(result);

            return RedirectToAction("ShoppingCart");
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
