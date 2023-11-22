using KittyShop.Data.DBContext;
using KittyShop.Data.Entities;
using KittyShop.Interfaces.IRepositories;
using KittyShop.Models;
using KittyShop.Services.Utility;
using Microsoft.EntityFrameworkCore;

namespace KittyShop.Repositories
{
    public class HomeRepository: IHomeRepository
    {
        private readonly KittyShopContext _context;
        public HomeRepository(KittyShopContext context) 
        {
            _context = context;
        }

        public IQueryable<Product> GetAllProductsAsQueryable()
        {
            return _context.Products;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            return await SaveChangesAsync();
        }

        public async Task<bool> UserNameExistsAsync(string userName)
        {
            return await _context.Users.AnyAsync(x => x.UserName == userName);
        }

        public async Task<User?> FindUserByIdAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<User?> FindUserByNameAsync(string userName)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.UserName == userName);
        }
    }
}
