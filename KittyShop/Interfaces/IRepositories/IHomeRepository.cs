using KittyShop.Data.Entities;
using KittyShop.Models;
using KittyShop.Services.Utility;

namespace KittyShop.Interfaces.IRepositories
{
    public interface IHomeRepository
    {
        Task<PaginatedList<CatModel>> GetProducts(string furrColor, string eyesColor,
            string description, string race, int? pageNumber, int pageSize);

        Task<bool> SaveChangesAsync();
        Task <bool> CreateUserAsync(User user);
        Task<bool> UserNameExistsAsync(string userName);
        Task<User?> FindUserByIdAsync(int userId);
        Task<User?> FindUserByNameAsync(string userName);
    }
}
