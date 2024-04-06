using KittyShop.Data.Entities;

namespace KittyShop.Interfaces.IRepositories
{
    public interface IAdminRepository
    {
        Task<bool> SaveChangesAsync();
        Task<bool> AddProductAsync(Product product);
        Task<Product?> FindProductByIdAsync(int productId);
        Task<bool> DeleteProductAsync(Product product);

    }
}
