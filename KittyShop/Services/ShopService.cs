using AutoMapper;
using KittyShop.Data.Entities;
using KittyShop.Interfaces.IRepositories;
using KittyShop.Interfaces.IServices;
using KittyShop.Models;
using KittyShop.Repositories;
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

        public async Task<(bool isAdded, string message)> AddProductToCartAsync(int userId, int productId)
        {
            string message = "";
            if (!await _shopRepository.CheckIfShoppingCartExistForUserAsync(userId))
                await CreateCartForUserAsync(userId);

            var shoppingCartId = await _shopRepository.GetShopingCartIdByUserIdAsync(userId);

            bool isAdded = await _shopRepository.AddItemToCartAsync(new CartItem() { ShoppingCartId = shoppingCartId!, ProductId = productId! });
            if (isAdded)
                message = MessagesConstants.ItemAddedToCartSuccess;
            return (isAdded, message);
        }

        public async Task CreateCartForUserAsync(int userId)
        {
            var cartForUser = new ShoppingCart() { UserId = userId! };

            bool isAdded = await _shopRepository.AddShoppingKartAsync(cartForUser);
        }

        public async Task<List<CartItemModel>> GetShoppingCartForUser(int userId)
        {
            var catList = new List<CatModel>();
            var cart = await _shopRepository.FindShopingCartByUserIdAsync(userId);

            foreach (var item in cart.CartItems)
            {
                var catModel = _mapper.Map<CatModel>(item.Product);
                catList.Add(catModel);
            }
            
            
            var itemList = catList
                .GroupBy(p => p.ProductId)
                .Select(g => new CartItemModel()
                {
                    Cat = g.First(), 
                    Quantity = g.Count() 
                }).ToList();


            return itemList;
        }
    }
}
