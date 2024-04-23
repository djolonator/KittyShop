using AutoMapper;
using KittyShop.Data.Entities;
using KittyShop.Interfaces.IRepositories;
using KittyShop.Interfaces.IServices;
using KittyShop.Models;
using KittyShop.Utility;

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

        public async Task<MessageModel> AddProductAsync(CatModel productModel)
        {
            var result = new MessageModel();
            var imagePath = await _imageService.SaveProductImageToProjectFolder(productModel.Image!);
            var productEntity = _mapper.Map<Product>(productModel);
            productEntity.ImgUrlPath = imagePath;

            result.IsSuccess = await _adminRepository.AddProductAsync(productEntity);

            result.Message = result.IsSuccess ? MessagesConstants.ProductAddSuccess : MessagesConstants.ProductAddFail;

            return result;  
        }

        public async Task<MessageModel> EditProductAsync(CatModel product)
        {
            var result = new MessageModel();
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

                result.IsSuccess = await _adminRepository.SaveChangesAsync();
                result.Message = result.IsSuccess ? MessagesConstants.ProductEditSuccess : MessagesConstants.ProductEditFail;
            }
            else
                result.Message = MessagesConstants.ProductDoesNotExist;

            return result;
        }

        public async Task<(CatModel? product, string message)> FindProductAsync(int productId)
        {
            string message = string.Empty;
            var entity = await _adminRepository.FindProductByIdAsync(productId);
            var product = _mapper.Map<CatModel>(entity);

            message = entity == null ? MessagesConstants.ProductDoesNotExist : message;

            return (product, message);
        }

        public async Task<MessageModel> DeleteProductAsync(int productId)
        {
            var result = new MessageModel();
            result.IsSuccess = await _adminRepository.DeleteProductAsync(new Product() { ProductId = productId});

            result.Message = result.IsSuccess ? MessagesConstants.ProductDeleteSuccess: MessagesConstants.ProductDeleteFail;

            return result;
        }
    }
}
