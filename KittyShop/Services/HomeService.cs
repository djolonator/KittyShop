using AutoMapper;
using KittyShop.Data.Entities;
using KittyShop.Interfaces.IRepositories;
using KittyShop.Interfaces.IServices;
using KittyShop.Models;
using KittyShop.Utility;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

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
            var allProducts = _homeRepository.GetAllProductsAsQueryable();
            allProducts = AddFilters(furrColor, eyesColor, description, race, allProducts);
            var paginatedProductsModel = ConvertEntitiesToModels(allProducts);
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
            }
            
            return (model, message);
        }

        public async Task<(bool isEdited, string message)> EditProfile(EditProfileModel userToEdit, int userId)
        {
            string message = MessagesConstants.SomethingWentWrong;
            bool isEdited = false;
            var entityToUpdate = await _homeRepository.FindUserByIdAsync(userId);
            if (entityToUpdate != null)
            {
                if (userToEdit.NewUserName != null)
                {
                    var userNameExists = await _homeRepository.UserNameExistsAsync(userToEdit.NewUserName);

                    if (userNameExists)
                    {
                        message = MessagesConstants.UserNameTaken;
                    }
                    else
                    {
                        userToEdit!.UserName = userToEdit.NewUserName!;
                    }
                }

                if (userToEdit.NewPassword != null)
                    userToEdit!.Password = _cipherService.Encrypt(userToEdit.NewPassword);

                if (userToEdit.NewEmail != null)
                    userToEdit!.Email = userToEdit.NewEmail;
            }

            _mapper.Map(userToEdit, entityToUpdate);

            isEdited = await _homeRepository.SaveChangesAsync();

            return (isEdited, message);
        }
    }
}
