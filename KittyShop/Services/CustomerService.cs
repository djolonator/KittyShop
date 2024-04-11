using AutoMapper;
using KittyShop.Data.Entities;
using KittyShop.Interfaces.IRepositories;
using KittyShop.Interfaces.IServices;
using KittyShop.Models;
using KittyShop.Utility;

namespace KittyShop.Services
{
    public class CustomerService: ICustomerService
    {
        private readonly CipherService _cipherService;
        private readonly IMapper _mapper;
        private readonly ICustomerRepository _userRepository;
        public CustomerService(CipherService cipherService, IMapper mapper, ICustomerRepository repository) 
        {
            _cipherService = cipherService;
            _mapper = mapper;
            _userRepository = repository;
        }
    }
}
