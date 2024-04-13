using KittyShop.Data.Entities;

namespace KittyShop.Interfaces.IRepositories
{
    public interface IShopRepository
    {
        Task<bool> SaveChangesAsync();
        Task<bool> AddShoppingKartAsync(ShoppingKart cartForUser);
        Task<ShoppingKart?> FindShopingCartByUserIdAsync(int userId);
        Task<bool> CheckIfShoppingCartExistForUserAsync(int userId);
        Task<bool> AddItemToCartAsync(KartItem item);
    }
}
