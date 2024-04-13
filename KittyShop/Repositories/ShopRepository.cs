using KittyShop.Data.DBContext;
using KittyShop.Data.Entities;
using KittyShop.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace KittyShop.Repositories
{
    public class ShopRepository: IShopRepository
    {
        private readonly KittyShopContext _context;
        public ShopRepository(KittyShopContext context)
        {
            _context = context;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() > 0);
        }

        public async Task<bool> AddShoppingKartAsync(ShoppingKart cartForUser)
        {
            await _context.ShoppingKarts.AddAsync(cartForUser);
            return await SaveChangesAsync();
        }

        public async Task<ShoppingKart?> FindShopingCartByUserIdAsync(int userId)
        {
            return await _context.ShoppingKarts.FirstOrDefaultAsync(k => k.User.UserId == userId);
        }

        public async Task<bool> CheckIfShoppingCartExistForUserAsync(int userId)
        {
            return await _context.ShoppingKarts.AnyAsync(k => k.User.UserId == userId);
        }

        public async Task<bool> AddItemToCartAsync(KartItem item)
        {
            await _context.KartItems.AddAsync(item);
            return await SaveChangesAsync();
        }
    }
}
