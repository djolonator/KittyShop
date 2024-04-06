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

            var isSaved = await _adminRepository.AddProductAsync(productEntity);

            return isSaved ? MessagesConstants.ProductAddSuccess : MessagesConstants.ProductAddFail;
        }

        public async Task<string> EditProductAsync(CatModel product)
        {
            string message = string.Empty;
            var entityToUpdate = await _adminRepository.FindProductByIdAsync(product.ProductId);

            if (entityToUpdate != null)
            {
                if (product.Image != null)
                {
                    if (_imageService.ProductImageExists(product.ImgUrlPath!))
                        _imageService.DeleteProductImage(product.ImgUrlPath!);

                    product.ImgUrlPath = await _imageService.SaveProductImageToProjectFolder(product.Image);
                }

                _mapper.Map(product, entityToUpdate);

                var isEdited = await _adminRepository.SaveChangesAsync();
                    message = isEdited ? MessagesConstants.ProductEditSuccess : MessagesConstants.ProductEditFail;
            }
            else
                message = MessagesConstants.ProductDoesNotExist;

            return message;
        }

        public async Task<(CatModel? product, string message)> FindProductAsync(int productId)
        {
            string message = string.Empty;
            var entity = await _adminRepository.FindProductByIdAsync(productId);
            var product = _mapper.Map<CatModel>(entity);

            if (entity == null)
                message = MessagesConstants.ProductDoesNotExist;

            return (product, message);
        }

        public async Task<string> DeleteProductAsync(int productId)
        {
            string message = string.Empty;
            var isDeleted = await _adminRepository.DeleteProductAsync(new Product() { ProductId = productId});

            if (isDeleted)
                message = MessagesConstants.ProductDeleteSuccess;
            else message = MessagesConstants.ProductDeleteFail;

            return message;
        }
    }
}
