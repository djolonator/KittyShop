using KittyShop.Data.Entities;
using KittyShop.Models;

namespace KittyShop.Interfaces.IServices
{
    public interface IShopService
    {
        Task CreateCartForUserAsync(int userId);
        Task<MessageModel> AddProductToUserCartAsync(int userId, int productId);
        Task<ShoppingCartModel> GetShoppingCartForUser(int userId);
        Task<MessageModel> UpdateCartForUser(int cartId, int productId, int quantity);
        Task<int> GetNumberOfItemsFromCart(int userId);
        int GetNumberOfItemsInCart(ShoppingCartModel cart);
        Task<MessageModel> Checkout(int cartId);
    }
}
