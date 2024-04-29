using KittyShop.Interfaces.IServices;
using KittyShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KittyShop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAdminService _adminService;
        public AdminController(ILogger<HomeController> logger, IAdminService adminService) 
        {
            _adminService = adminService;
            _logger = logger;
        }
        
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public IActionResult AddShopItem()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddShopItem(CatModel cat)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _adminService.AddProductAsync(cat);
                    SetMessageForUser(result);
                }
                else
                    return View(cat);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Product could not be added. Code exited with message {ex.Message} at {ex.StackTrace}");
                SetMessageForUser(new MessageModel() { Message = "Something went wrong with request." });
            }
            
            return RedirectToAction("ShopItemList", "Home");
        }
      
        public async Task<IActionResult> EditShopItem(int productId)
        {
            string message = "";
            try
            {
                var result = await _adminService.FindProductAsync(productId);
                message = result.message;

                if (result.product != null)
                    return View(result.product);
                else SetMessageForUser(new MessageModel() { Message = message });
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Edit product page failed to load. Code exited with message {ex.Message} at {ex.StackTrace}");
                SetMessageForUser(new MessageModel() { Message = "Something went wrong with request." });
            }
            
            return RedirectToAction("ShopItemList", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> EditShopItem(CatModel product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _adminService.EditProductAsync(product);
                    SetMessageForUser(result);
                }
                else
                    return View(product);
            }
            catch(Exception ex)
            {
                _logger.LogCritical($"Edit product failed. Code exited with message {ex.Message} at {ex.StackTrace}");
                SetMessageForUser(new MessageModel() { Message = "Something went wrong with request." });
            }
            
            return RedirectToAction("ShopItemList", "Home");
        }

        public async Task<IActionResult> DeleteProduct(int productId)
        {
            try
            {
                var result = await _adminService.DeleteProductAsync(productId);
                SetMessageForUser(result);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Edit delete product failed. Code exited with message {ex.Message} at {ex.StackTrace}");
            }
            
            return RedirectToAction("ShopItemList", "Home");
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
