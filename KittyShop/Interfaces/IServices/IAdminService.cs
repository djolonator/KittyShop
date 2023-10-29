using KittyShop.Data.Entities;
using KittyShop.Models;
using KittyShop.Services.Utility;

namespace KittyShop.Interfaces.IServices
{
    public interface IAdminService
    {
        Task<string> AddProductAsync(CatModel product);
        Task<(CatModel product, string message)> EditProductAsync(CatModel product);
        Task<(CatModel? product, string message)> FindProductAsync(int productId);

    }
}
