using System.ComponentModel.DataAnnotations;


namespace KittyShop.Services.Utility
{
    public class ValidationForImage : ValidationAttribute
    {
        private readonly string _imgUrlPath;

        public ValidationForImage(string imgUrlPath)
        {
            _imgUrlPath = imgUrlPath;
        }
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var imgPath = string.Empty;
            var prop = validationContext.ObjectInstance.GetType().GetProperty(_imgUrlPath);
            
            if (prop != null)
                imgPath = prop.GetValue(validationContext.ObjectInstance)!.ToString();
            
            if (prop == null)
            {
                return new ValidationResult($"Unknown property {_imgUrlPath}");
            }
           
            int width = 0;
            int height = 0;

            if (value == null && imgPath == null)
            {
                return new ValidationResult("You must upload image for product");
            }

            var file = value as IFormFile;

            if (file != null)
            {
                using (var image =  Image.Load(file.OpenReadStream()))
                {
                    width = image.Width;
                    height = image.Height;
                }
            }

            if (width != height)
            {
                return new ValidationResult("Ratio must be 1:1");
            }
            else
                return ValidationResult.Success!;
        }
    }
}
