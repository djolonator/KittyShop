
namespace KittyShop.Services.Utility
{
    public class ImageSevice
    {
        private readonly IWebHostEnvironment _env;
        private readonly string productImageFolderName = "Products Images";

        public ImageSevice(IWebHostEnvironment webHostEnvironment)
        {
            _env = webHostEnvironment;
        }

        public async Task<string> SaveProductImageToProjectFolder(IFormFile image)
        {
            var imageName = MakeImageName(image);
            var path = Path.Combine(_env.WebRootPath, productImageFolderName, imageName);
            
            using var stream = new FileStream(path, FileMode.Create);
            await image.CopyToAsync(stream);

            return $"{productImageFolderName}/{imageName}";
        }

        private string MakeImageName(IFormFile image)
        {
            var imageExtension = System.IO.Path.GetExtension(image.FileName);
            var imageName = $"cat-{Guid.NewGuid()}{imageExtension}";

            return imageName;
        }

        public bool ProductImageExists(string path)
        {
            
            var wholePath = Path.Combine(_env.WebRootPath, path);
            var doesExist = File.Exists(wholePath);
            return doesExist;
        }

        public void DeleteProductImage(string path)
        {
            var wholePath = Path.Combine(_env.WebRootPath, path);
            File.Delete(wholePath);
        }
    }
}
