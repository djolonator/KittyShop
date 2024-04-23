using KittyShop.Models;


namespace KittyShop.Interfaces.IServices
{
    public interface IAdminService
    {
        Task<MessageModel> AddProductAsync(CatModel product);
        Task<MessageModel> DeleteProductAsync(int productId);
        Task<MessageModel> EditProductAsync(CatModel product);
        Task<(CatModel? product, string message)> FindProductAsync(int productId);

    }
}
