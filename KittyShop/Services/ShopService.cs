using KittyShop.Data.Entities;
using KittyShop.Interfaces.IRepositories;
using KittyShop.Interfaces.IServices;
using KittyShop.Repositories;
using KittyShop.Utility;

namespace KittyShop.Services
{
    public class ShopService: IShopService
    {
        private readonly IShopRepository _shopRepository;
        private readonly IHomeRepository _homeRepository;
        private readonly IAdminRepository _adminRepository;
        public ShopService(IShopRepository shopRepository, IHomeRepository homeRepository, IAdminRepository adminRepository)
        {
            _shopRepository = shopRepository;
            _homeRepository = homeRepository;
            _adminRepository = adminRepository;
        }

        public async Task<(bool isAdded, string message)> AddProductToCartAsync(int userId, int productId)
        {
            string message = "";
            if (!await _shopRepository.CheckIfShoppingCartExistForUserAsync(userId))
                await CreateCartForUserAsync(userId);

            var shoppingCart = await _shopRepository.FindShopingCartByUserIdAsync(userId);
            var product = await _adminRepository.FindProductByIdAsync(productId);
            bool isAdded = await _shopRepository.AddItemToCartAsync(new CartItem() {ShoppingCartId = shoppingCart.ShoppingCartId!, ProductId = productId! });
            if (isAdded)
                message = MessagesConstants.ItemAddedToCartSuccess;
            return (isAdded, message);
        }

        public async Task CreateCartForUserAsync(int userId)
        {
            var user = await _homeRepository.FindUserByIdAsync(userId);
            var cartForUser = new ShoppingCart() { UserId = userId! };

            bool isAdded = await _shopRepository.AddShoppingKartAsync(cartForUser);
        }
    }
}
