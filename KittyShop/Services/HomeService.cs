using AutoMapper;
using KittyShop.Data.Entities;
using KittyShop.Interfaces.IRepositories;
using KittyShop.Interfaces.IServices;
using KittyShop.Models;
using KittyShop.Utility;
using Microsoft.EntityFrameworkCore;

namespace KittyShop.Services
{
    public class HomeService : IHomeService
    {
        private readonly IHomeRepository _homeRepository;
        private readonly IShopRepository _shopRepository;
        private readonly CipherService _cipherService;
        private readonly IMapper _mapper;

        public HomeService(IHomeRepository homeRepository, CipherService cipherService, IMapper mapper, IShopRepository shopRepository)
        {
            _homeRepository = homeRepository;
            _cipherService = cipherService;
            _mapper = mapper;
            _shopRepository = shopRepository;
        }
        public async Task<PaginatedList<CatModel>> GetProductsAsync(string furrColor, string eyesColor,
            string description, string race, int? pageNumber, int pageSize)
        {
            var products = _homeRepository.GetAllProductsAsQueryable();
            products = AddFilters(furrColor, eyesColor, description, race, products);
            var paginatedProductsModel = ConvertEntitiesToModels(products);
            var listToReturn = await PaginatedList<CatModel>.CreateAsync(paginatedProductsModel.AsNoTracking(), pageNumber ?? 1, pageSize);

            return listToReturn;
        }

        private IQueryable<Product> AddFilters(string furrColor, string eyesColor,
            string description, string race, IQueryable<Product> allProducts)
        {
            allProducts = AddToQueryFurrFilter(furrColor, allProducts);
            allProducts = AddToQueryEyesFilter(eyesColor, allProducts);
            allProducts = AddToQueryDescriptionFilter(description, allProducts);
            allProducts = AddToQueryRaceFilter(race, allProducts);
            return allProducts;
        }

        private IQueryable<CatModel> ConvertEntitiesToModels(IQueryable<Product> products)
        {
            return products
                .Select(p => new CatModel
                {
                    ProductId = p.ProductId,
                    Race = p.Race,
                    FurrColor = p.FurrColor,
                    Description = p.Description,
                    Price = p.Price,
                    EyesColor = p.EyesColor,
                    ImgUrlPath = p.ImgUrlPath
                }).AsQueryable();
        }
        private IQueryable<Product> AddToQueryFurrFilter(string furrColor, IQueryable<Product> products)
        {
            if (furrColor != null)
            {
                furrColor = furrColor.Trim();
                products = products.Where(p => p.FurrColor.Contains(furrColor));
            }
            return products;
        }

        private IQueryable<Product> AddToQueryEyesFilter(string eyesColor, IQueryable<Product> products)
        {
            if (eyesColor != null)
            {
                eyesColor = eyesColor.Trim();
                products = products.Where(p => p.EyesColor.Contains(eyesColor));
            }
            return products;
        }

        private IQueryable<Product> AddToQueryDescriptionFilter(string description, IQueryable<Product> products)
        {
            if (description != null)
            {
                description = description.Trim();
                products = products.Where(p => p.Description.Contains(description));
            }
            return products;
        }

        private IQueryable<Product> AddToQueryRaceFilter(string race, IQueryable<Product> products)
        {
            if (race != null)
            {
                race = race.Trim();
                products = products.Where(p => p.Race.Contains(race));
            }
            return products;
        }

        public async Task<MessageModel> RegisterUser(RegisterModel model)
        {
            var result = new MessageModel();
            if (await _homeRepository.UserNameExistsAsync(model.UserName))
                result.Message = MessagesConstants.UserNameTaken;
            else
            {
                result.IsSuccess = await CreateUser(model);
                if (result.IsSuccess)
                    result.Message = MessagesConstants.RegisterSuccess;
                else
                    result.Message = MessagesConstants.RegisterFail;
            }

            return result;
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

        public async Task<(LoginModel? userModel, string message)> Login(LoginModel model)
        {
            var message = "";
            var isAuthenticated = await AuthenticateUser(model.UserName, model.Password);

            if (!isAuthenticated.verdict)
            {
                model = new LoginModel();
                message = MessagesConstants.WrongLoginCredentials;
            }
            else
                model = _mapper.Map<LoginModel>(isAuthenticated.user);

            return (model, message);
        }

        private async Task<(bool verdict, User? user)> AuthenticateUser(string userName, string password)
        {
            bool verdict = false;
            var user = await _homeRepository.FindUserByNameAsync(userName);

            if (user != null && _cipherService.Decrypt(user!.Password) == password)
                verdict = true;
            else
                verdict = false;

            return (verdict, user);
        }

        public async Task<EditProfileModel> GetUserAsync(int userId)
        {
            var userEntity = await _homeRepository.FindUserByIdAsync(userId);
            var userModel = _mapper.Map<EditProfileModel>(userEntity);

            return userModel;
        }

        public async Task<MessageModel> EditProfile(EditProfileModel userToEdit, int userId)
        {
            var result = new MessageModel();
            bool isPasswordChange = userToEdit.NewPassword != null;
            bool isEmailChange = userToEdit.NewEmail != null;
            bool isNameChange = userToEdit.NewUserName != null;
            bool isNameChangeValid = false;
            bool isChanged = isNameChange || isEmailChange || isPasswordChange;

            if (isChanged)
            {
                if (isPasswordChange)
                    userToEdit!.Password = _cipherService.Encrypt(userToEdit.NewPassword!);
                if (isEmailChange)
                    userToEdit!.Email = userToEdit.NewEmail;
                if (isNameChange)
                {
                    isNameChangeValid = !await _homeRepository.UserNameExistsAsync(userToEdit.NewUserName!);

                    if (isNameChangeValid)
                        userToEdit!.UserName = userToEdit.NewUserName!;
                    else
                        result.Message = MessagesConstants.UserNameTaken;
                }

                if ((!isNameChange || isNameChangeValid) || (isEmailChange || isPasswordChange))
                {
                    var entityToUpdate = await _homeRepository.FindUserByIdAsync(userId);
                    _mapper.Map(userToEdit, entityToUpdate);
                    result.IsSuccess = await _homeRepository.SaveChangesAsync();
                    if (result.IsSuccess)
                        result.Message = MessagesConstants.UserEditSuccess;
                }
            }
            else
            {
                result.IsSuccess = true;
                result.Message = MessagesConstants.UserEditNoChange;
            }

            return result;
        }
    } 
}
