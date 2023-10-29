using AutoMapper;
using KittyShop.Data.Entities;
using KittyShop.Interfaces.IRepositories;
using KittyShop.Interfaces.IServices;
using KittyShop.Models;
using KittyShop.Services.Utility;

namespace KittyShop.Services
{
    public class AdminService: IAdminService
    {
        private readonly ImageSevice _imageService;
        private readonly IMapper _mapper;
        private readonly IAdminRepository _adminRepository;
        
        public AdminService(ImageSevice imageService, IMapper mapper, IAdminRepository adminRepository) 
        {
            _imageService = imageService;
            _mapper = mapper;
            _adminRepository = adminRepository;
        }

        public async Task<string> AddProductAsync(CatModel productModel)
        {
            var imagePath = await _imageService.SaveProductImageToProjectFolder(productModel.Image!);
            var productEntity = _mapper.Map<Product>(productModel);
            productEntity.ImgUrlPath = imagePath;

            await _adminRepository.AddProductAsync(productEntity);

            return "bla";
        }

        public async Task<(CatModel product, string message)> EditProductAsync(CatModel product)
        {
            var message = string.Empty;

            if (product.Image != null)
            {
                if (_imageService.ProductImageExists(product.ImgUrlPath!))
                    _imageService.DeleteProductImage(product.ImgUrlPath!);

                product.ImgUrlPath = await _imageService.SaveProductImageToProjectFolder(product.Image);
            }

            var entityToUpdate = _adminRepository.FindProductByIdAsync(product.ProductId);

            if (entityToUpdate != null)
            {
                await _mapper.Map(product, entityToUpdate);
                if (!await _adminRepository.SaveChangesAsync())
                    message = "There was an error editing product";
            }

            return (product, message);
        }

        public async Task<(CatModel? product, string message)> FindProductAsync(int productId)
        {
            var message = string.Empty;
            var entity = await _adminRepository.FindProductByIdAsync(productId);
            var product = _mapper.Map<CatModel>(entity);

            if (product == null)
                message = "Product could not be found";

            return (product, message);
        }
        
    }
}
