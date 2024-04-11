using AutoMapper.Configuration.Annotations;
using KittyShop.Data.Entities;
using KittyShop.Interfaces.IServices;
using KittyShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KittyShop.Controllers
{
    //[Authorize(Policy = "Admin")]
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
            var roleClaim = User.Claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault();
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
                    var message = await _adminService.AddProductAsync(cat);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Product could not be added. Code exited with message {ex.Message} at {ex.StackTrace}");
            }
            //result nosi poruku za korisnika sweetalert
            return RedirectToAction("ShopItemList", "Home");
        }
      
        public async Task<IActionResult> EditShopItem(int productId)
        {
            try
            {
                var result = await _adminService.FindProductAsync(productId);

                if (result.product != null)
                {
                    return View(result.product);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Edit product page failed to load. Code exited with message {ex.Message} at {ex.StackTrace}");
            }
            //result.message nosi poruku za korisnika sweetalert
            return RedirectToAction("ShopItemList", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> EditShopItem(CatModel product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var message = await _adminService.EditProductAsync(product);
                }
            }
            catch(Exception ex)
            {
                _logger.LogCritical($"Edit product failed. Code exited with message {ex.Message} at {ex.StackTrace}");
            }
            //result nosi poruku za korisnika
            return RedirectToAction("ShopItemList", "Home");
        }

        public async Task<IActionResult> DeleteProduct(int productId)
        {
            try
            {
                var message = await _adminService.DeleteProductAsync(productId);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Edit delete product failed. Code exited with message {ex.Message} at {ex.StackTrace}");
            }
            //result nosi poruku za korisnika
            return RedirectToAction("ShopItemList", "Home");
        }
    }
}
