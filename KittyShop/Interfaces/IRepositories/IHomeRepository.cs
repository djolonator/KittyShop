using KittyShop.Data.Entities;


namespace KittyShop.Interfaces.IRepositories
{
    public interface IHomeRepository
    {
        public IQueryable<Product> GetAllProductsAsQueryable();
        Task<bool> SaveChangesAsync();
        Task <bool> CreateUserAsync(User user);
        Task<bool> UserNameExistsAsync(string userName);
        Task<User?> FindUserByIdAsync(int userId);
        Task<User?> FindUserByNameAsync(string userName);
    }
}
