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

        public async Task<bool> AddShoppingKartAsync(ShoppingCart cartForUser)
        {
            await _context.ShoppingCarts.AddAsync(cartForUser);
            return await SaveChangesAsync();
        }

        public async Task<int> GetShopingCartIdByUserIdAsync(int userId)
        {
            var cart =  await _context.ShoppingCarts.FirstOrDefaultAsync(k => k.UserId == userId);
            return cart!.ShoppingCartId;
        }

        public async Task<ShoppingCart?> FindShopingCartByUserIdAsync(int userId)
        {
            return await _context.ShoppingCarts.Include(c => c.CartItems)
                                               .ThenInclude(c => c.Product)
                                               .FirstOrDefaultAsync(k => k.UserId == userId);

        }

        public async Task<ShoppingCart?> GetShoppingCartByIdAsync(int cartId)
        {
            return await _context.ShoppingCarts.Include(c => c.CartItems)
                                               .ThenInclude(c => c.Product)
                                               .FirstOrDefaultAsync(c=> c.ShoppingCartId == cartId);
        }

        public async Task<bool> CheckIfShoppingCartExistForUserAsync(int userId)
        {
            return await _context.ShoppingCarts.AnyAsync(k => k.UserId == userId);
        }

        public async Task<bool> AddItemToCartAsync(CartItem item)
        {
            await _context.CartItems.AddAsync(item);
            return await SaveChangesAsync();
        }

        public async Task<bool> IncreaseQuantityByOne(int shoppingCartId, int productId)
        {
            var cartItem = _context.CartItems.FirstOrDefault(c => c.ShoppingCartId == shoppingCartId && c.ProductId == productId);

            cartItem.Quantity = +1;

            return await SaveChangesAsync();
        }
    }
}
