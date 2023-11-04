using AutoMapper;
using KittyShop.Data.Entities;
using KittyShop.Interfaces.IRepositories;
using KittyShop.Interfaces.IServices;
using KittyShop.Models;
using KittyShop.Services.Utility;

namespace KittyShop.Services
{
    public class HomeService: IHomeService
    {
        private readonly IHomeRepository _homeRepository;
        private readonly CipherService _cipherService;
        private readonly IMapper _mapper;

        public HomeService(IHomeRepository homeRepository, CipherService cipherService, IMapper mapper) 
        {
            _homeRepository = homeRepository;
            _cipherService = cipherService;
            _mapper = mapper;
        }
        public async Task<PaginatedList<CatModel>> GetProductsAsync(string furrColor, string eyesColor,
            string description, string race, int? pageNumber, int pageSize)
        {
            var products = await _homeRepository.GetProducts(furrColor, eyesColor,
                    description, race, pageNumber, pageSize);

            return products;
        }

        public async Task RegisterUser(RegisterModel model)
        {
            var encryptedPassword = _cipherService.Encrypt(model.Password);
            model.Password = encryptedPassword;
            var entity = _mapper.Map<User>(model);
            entity.Type = Enums.UserTypes.Regular;
            await _homeRepository.CreateUserAsync(entity);
        }

        public async Task<(User user, string message)> Login(LoginModel model)
        {
            if (!await _homeRepository.UserNameExistsAsync(model.UserName))
                return (new User(), "User with that user name does not exist");

            var user = await _homeRepository.FindUserByNameAsync(model.UserName);

            if (_cipherService.Decrypt(user!.Password) != model.Password)
                return (new User(), $"Password for {user.UserName} is incorect");

            return (user, string.Empty);
        }

        public async Task<EditProfileModel> GetUserAsync(int userId)
        {
            var user = await _homeRepository.FindUserByIdAsync(userId);
            var model = _mapper.Map<EditProfileModel>(user);
            model.Password = _cipherService.Decrypt(model.Password!);
            return model;
        }

        public async Task<string> EditProfile(EditProfileModel userToEdit, int userId)
        {
            string result = string.Empty;
            var entityToUpdate = await _homeRepository.FindUserByIdAsync(userId);
            if (userToEdit.NewUserName != null)
            {
                var userNameExists = await _homeRepository.UserNameExistsAsync(userToEdit.NewUserName);

                if (userNameExists)
                {
                    return $"Username {userToEdit.NewUserName} is not available";
                }
                else
                {
                    userToEdit!.UserName = userToEdit.NewUserName!;
                }
            }

            if (userToEdit.NewPassword != null)
                userToEdit!.Password = _cipherService.Encrypt(userToEdit.NewPassword);
            else
                userToEdit!.Password = _cipherService.Encrypt(userToEdit.Password);

            if (userToEdit.NewEmail != null)
            {
                userToEdit!.Email = userToEdit.NewEmail;

                //napraviti metodu za findUserByEmail i proveriti da li postoji
            }
                

            _mapper.Map(userToEdit, entityToUpdate);

            await _homeRepository.SaveChangesAsync();


            return result;
            
        }
    }
}
