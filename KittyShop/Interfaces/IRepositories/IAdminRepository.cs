using KittyShop.Data.Entities;

namespace KittyShop.Interfaces.IRepositories
{
    public interface IAdminRepository
    {
        Task<bool> SaveChangesAsync();
        Task AddProductAsync(Product product);
        Task<Product?> FindProductByIdAsync(int productId);


    }
}
