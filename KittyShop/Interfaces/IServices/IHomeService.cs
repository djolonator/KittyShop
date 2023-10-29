﻿using KittyShop.Data.Entities;
using KittyShop.Models;
using KittyShop.Services.Utility;

namespace KittyShop.Interfaces.IServices
{
    public interface IHomeService
    {
        Task<PaginatedList<CatModel>> GetProductsAsync(string furrColor, string eyesColor,
            string description, string race, int? pageNumber, int pageSize);
        public Task RegisterUser(RegisterModel model);
        Task<(User user, string message)> Login(LoginModel model);
        Task<EditProfileModel> GetUserAsync(int userId);
        Task<string> EditProfile(EditProfileModel userToEdit, int userId);
    }
}
