﻿using KittyShop.Data.Entities;

namespace KittyShop.Interfaces.IRepositories
{
    public interface IShopRepository
    {
        Task<bool> SaveChangesAsync();
        Task<bool> AddShoppingKartAsync(ShoppingCart cartForUser);
        Task<ShoppingCart?> FindShopingCartByUserIdAsync(int userId);
        Task<bool> CheckIfShoppingCartExistForUserAsync(int userId);
        Task<bool> AddItemToCartAsync(CartItem item);
    }
}
