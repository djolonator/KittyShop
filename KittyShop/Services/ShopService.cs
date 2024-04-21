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

        public async Task<MessageModel> AddProductToUserCartAsync(int userId, int productId)
        {
            var result = new MessageModel();
            if (!await _shopRepository.CheckIfShoppingCartExistsForUserAsync(userId))
                await CreateCartForUserAsync(userId); // videti ovde da li pitati jel kreirano

            var cart = await _shopRepository.FindShopingCartByUserIdAsync(userId);

            if (IsProductAlradyInCart(cart!, productId))
                result = await ChangeQuantityOfItemInCartAsync(cart!, productId);
            else
                result = await AddProductToCartAsync(cart!, productId);

            return result;
        }

        private bool IsProductAlradyInCart(ShoppingCart cart, int productId)
        {
            return cart.CartItems.Any(i => i.ProductId == productId) ? true : false;
        }

        private async Task<MessageModel> ChangeQuantityOfItemInCartAsync(ShoppingCart cart, int productId)
        {
            var result = new MessageModel();
            var item = cart!.CartItems.FirstOrDefault(i => i.ProductId == productId)!;

            if (item.Quantity < 10)
            {
                item.Quantity++;
                result.IsSuccess = await _shopRepository.SaveChangesAsync();
                if (result.IsSuccess)
                    result.Message = MessagesConstants.ItemAddedToCartSuccess;
                else
                    result.Message = MessagesConstants.FailedToAddItemToCart;
            }
            else
                result.Message = MessagesConstants.CartItemMaximumQuantity;

            return result;
        }

        private async Task<MessageModel> AddProductToCartAsync(ShoppingCart cart, int productId)
        {
            var result = new MessageModel();
            cart!.CartItems.Add(new CartItem() { ShoppingCartId = cart.ShoppingCartId!, ProductId = productId! });
            result.IsSuccess = await _shopRepository.SaveChangesAsync();
            if (result.IsSuccess)
                result.Message = MessagesConstants.ItemAddedToCartSuccess;
            else
                result.Message = MessagesConstants.FailedToAddItemToCart;

            return result;
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
            var result = new MessageModel();

            var cart = await _shopRepository.GetShoppingCartByIdAsync(cartId);
            var cartItem = cart!.CartItems.FirstOrDefault(c => c.ProductId == productId)!;

            if (quantity > 0)
                cartItem.Quantity = quantity;
            else
                cart.CartItems.Remove(cartItem);

            result.IsSuccess = await _shopRepository.SaveChangesAsync();
            if (result.IsSuccess)
                result.Message = MessagesConstants.CartUpdateSuccess;
            else
                result.Message = MessagesConstants.FailedToUpdateCart;

            return result;
        }
    }
}
