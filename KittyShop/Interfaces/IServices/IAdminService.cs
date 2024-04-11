using KittyShop.Models;


namespace KittyShop.Interfaces.IServices
{
    public interface IAdminService
    {
        Task<string> AddProductAsync(CatModel product);
        Task<string> DeleteProductAsync(int productId);
        Task<string> EditProductAsync(CatModel product);
        Task<(CatModel? product, string message)> FindProductAsync(int productId);

    }
}
