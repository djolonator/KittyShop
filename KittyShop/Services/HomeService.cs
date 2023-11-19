using AutoMapper;
using KittyShop.Data.Entities;
using KittyShop.Interfaces.IRepositories;
using KittyShop.Interfaces.IServices;
using KittyShop.Models;
using KittyShop.Services.Utility;

namespace KittyShop.Services
{
    public class HomeService : IHomeService
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

        public async Task<(bool isRegisterSuccess, string message)> RegisterUser(RegisterModel model)
        {
            bool isAuthenticated = await AuthenticateRegister(model);
            bool isCreated = await CreateUser(model);
            bool isRegisterSuccess = false;
            string message = string.Empty;

            if (isAuthenticated && isCreated)
            {
                isRegisterSuccess = true;
                message = MessagesConstants.RegisterSuccess;
            }
            else if (!isAuthenticated)
                message = MessagesConstants.UserNameTaken;

            return (isRegisterSuccess, message);    
        }

        private async Task<bool> AuthenticateRegister(RegisterModel model)
        {
            return !await _homeRepository.UserNameExistsAsync(model.UserName);
        }

        private async Task<bool> CreateUser(RegisterModel model)
        {
            model.Password = SetPasswordForRegister(model.Password);
            model.Type = Enums.UserTypes.Regular;
            var entity = _mapper.Map<User>(model);

            return await _homeRepository.CreateUserAsync(entity);
        }

        private string SetPasswordForRegister(string password)
        {
            return _cipherService.Encrypt(password);
        }

        public async Task<(User? user, string message)> Login(LoginModel model)
        {
            var message = MessagesConstants.WrongLoginCredentials;
            var isAuthenticated = await AuthenticateLogin(model.UserName, model.Password);

            if (isAuthenticated.verdict)
                message = MessagesConstants.LoggedInSuccess;

            return (isAuthenticated.user, message);
        }

        private async Task<(bool verdict, User? user)> AuthenticateLogin(string userName, string password)
        {
            bool verdict = false;
            var user = await _homeRepository.FindUserByNameAsync(userName);

            if (user != null && _cipherService.Decrypt(user!.Password) == password)
                verdict = true;
            else
                verdict = false;

            return (verdict, user);
        }

        public async Task<(EditProfileModel user, string message)> GetUserAsync(int userId)
        {
            string message = string.Empty;
            var model = new EditProfileModel();
            var user = await _homeRepository.FindUserByIdAsync(userId);
            if (user == null)
                message = MessagesConstants.SomethingWentWrong;
            else
            {
                model = _mapper.Map<EditProfileModel>(user);
                model.Password = _cipherService.Decrypt(model.Password!);
            }
            
            return (model, message);
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
