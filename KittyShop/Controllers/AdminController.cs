using AutoMapper.Configuration.Annotations;
using KittyShop.Interfaces.IServices;
using KittyShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KittyShop.Controllers
{
    //[Authorize(Policy = "Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService) 
        {
            _adminService = adminService;
        }
        
        public async Task<IActionResult> Index()
        {
            //var nameclaim = User.Claims.FirstOrDefault(c => c.Type == "Name");
            //var shopclaim = User.Claims.FirstOrDefault(c => c.Type == "Shop");
            //var adminclaim = User.Claims.FirstOrDefault(c => c.Type == "Admin");

            var roleClaim = User.Claims.FirstOrDefault(c => c.Type == "Role");
            return View();
        }

        public IActionResult AddShopItem()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddShopItem(CatModel cat)
        {
            if (ModelState.IsValid)
            {
                var result = await _adminService.AddProductAsync(cat);
                return RedirectToAction("ShopItemList", "Home");
            }    

            return View(cat);
        }
      
        public async Task<IActionResult> EditShopItem(int productId)
        {
            var product = await _adminService.FindProductAsync(productId);

            if (product.product == null)
            {
                TempData["Message"] = product.message;
            }

            return View(product.product);
        }

        [HttpPost]
        public async Task<IActionResult> EditShopItem(CatModel product)
        {
            if (ModelState.IsValid)
            {
                var result = await _adminService.EditProductAsync(product);
                ViewData["Message"] = result.message;
                product = result.product;
            }

            return View(product);
        }
    }
}
