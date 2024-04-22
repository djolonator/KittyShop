using KittyShop.Data.Entities;
using KittyShop.Models;
using KittyShop.Utility;

namespace KittyShop.Interfaces.IServices
{
    public interface IHomeService
    {
        Task<PaginatedList<CatModel>> GetProductsAsync(string furrColor, string eyesColor,
            string description, string race, int? pageNumber, int pageSize);
        public Task<MessageModel> RegisterUser(RegisterModel model);
        Task<(LoginModel? userModel, string message)> Login(LoginModel model);
        Task<EditProfileModel> GetUserAsync(int userId);
        Task<MessageModel> EditProfile(EditProfileModel userToEdit, int userId);
    }
}
