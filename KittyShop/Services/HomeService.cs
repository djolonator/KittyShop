﻿using AutoMapper;
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

        public async Task<MessageModel> RegisterUser(RegisterModel model)
        {
            var result = new MessageModel();
            if (await UsernameExists(model))
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

        private async Task<bool> UsernameExists(RegisterModel model)
        {
            return await _homeRepository.UserNameExistsAsync(model.UserName);
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
                message = MessagesConstants.WrongLoginCredentials;
            
            var userModel = _mapper.Map<LoginModel>(isAuthenticated.user);

            return (userModel, message);
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
            var entityToUpdate = await _homeRepository.FindUserByIdAsync(userId);

            if (userToEdit.NewPassword != null)
                userToEdit!.Password = _cipherService.Encrypt(userToEdit.NewPassword);

            if (userToEdit.NewEmail != null)
                userToEdit!.Email = userToEdit.NewEmail;

            if (userToEdit.NewUserName != null)
            {
                var userNameExists = await _homeRepository.UserNameExistsAsync(userToEdit.NewUserName);

                if (userNameExists)
                {
                    result.Message = MessagesConstants.UserNameTaken;
                }
                else
                {
                    userToEdit!.UserName = userToEdit.NewUserName!;
                    _mapper.Map(userToEdit, entityToUpdate);

                    result.IsSuccess = await _homeRepository.SaveChangesAsync();

                    if (result.IsSuccess)
                        result.Message = MessagesConstants.UserEditedSuccessfully;
                }
            }

            return result;
        }
    }
}
