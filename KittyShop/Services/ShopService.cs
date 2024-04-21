using AutoMapper;
using KittyShop.Data.Entities;
using KittyShop.Interfaces.IRepositories;
using KittyShop.Interfaces.IServices;
using KittyShop.Models;
using KittyShop.Utility;

namespace KittyShop.Services
{
    public class ShopService : IShopService
    {
        private readonly IShopRepository _shopRepository;
        private readonly IMapper _mapper;

        public ShopService(IShopRepository shopRepository, IMapper mapper)
        {
            _shopRepository = shopRepository;
            _mapper = mapper;
        }

        public async Task<MessageModel> AddProductToCartAsync(int userId, int productId)
        {
            var result = new MessageModel();
            if (!await _shopRepository.CheckIfShoppingCartExistForUserAsync(userId))
                await CreateCartForUserAsync(userId);

            var cart = await _shopRepository.FindShopingCartByUserIdAsync(userId);

            if (cart.CartItems.Any(i => i.ProductId == productId))
            {
                var item = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);

                // maks je 10, zbog selecta
                item.Quantity ++;
            }
            else
            {
                cart.CartItems.Add(new CartItem() { ShoppingCartId = cart.ShoppingCartId!, ProductId = productId! });
            }

            bool isSaved = await _shopRepository.SaveChangesAsync();
            if (isSaved)
                result.Message = MessagesConstants.ItemAddedToCartSuccess;

            return result;
        }

        private async Task<MessageModel> IncreaseQuantityOfItemInCart(int shoppingCartId, int productId)
        {
            string message = "";
            bool isAdded = await _shopRepository.IncreaseQuantityByOne(shoppingCartId, productId);
            if (isAdded)
                message = MessagesConstants.ItemAddedToCartSuccess;

            return (new MessageModel() { IsSuccess = isAdded, Message = message });
        }

        private async Task<MessageModel> AddProductToCart(int shoppingCartId, int productId)
        {
            
            string message = "";
            bool isAdded = await _shopRepository.AddItemToCartAsync(new CartItem() { ShoppingCartId = shoppingCartId!, ProductId = productId! });
            if (isAdded)
                message = MessagesConstants.ItemAddedToCartSuccess;

            return (new MessageModel() { IsSuccess = isAdded, Message = message});
        }

        public async Task CreateCartForUserAsync(int userId)
        {
            var cartForUser = new ShoppingCart() { UserId = userId! };

            bool isAdded = await _shopRepository.AddShoppingKartAsync(cartForUser);
        }

        public async Task<ShoppingCartModel> GetShoppingCartForUser(int userId)
        {
            var cartEntity = await _shopRepository.FindShopingCartByUserIdAsync(userId);
            float totalPrice = 0;

            var cartForView = _mapper.Map<ShoppingCartModel>(cartEntity);

            cartForView.CartItems.ForEach(cartItem => { totalPrice +=cartItem.Quantity * cartItem.Cat.Price; });
            cartForView.TotalPrice = totalPrice;
            return cartForView;
        }

        public async Task<MessageModel> UpdateCartForUser(int cartId, int productId, int quantity)
        {
            bool isUpdated = false;
            string message = "";

            var cart = await _shopRepository.GetShoppingCartByIdAsync(cartId);
            var cartItem = cart.CartItems.FirstOrDefault(c => c.ProductId == productId);

            if (quantity > 0)
            {
                cartItem.Quantity = quantity;
            }
            else
            {
                cart.CartItems.Remove(cartItem);
            }

            isUpdated = await _shopRepository.SaveChangesAsync(); 

            return (new MessageModel() { IsSuccess = isUpdated, Message = message });
        }
    }
}
