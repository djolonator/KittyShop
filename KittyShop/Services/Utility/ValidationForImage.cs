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

            var property = validationContext.ObjectType.GetProperty(_imgUrlPath);
            if (property == null)
            {
                return new ValidationResult($"Unknown property {_imgUrlPath}");
            }



            int width = 0;
            int height = 0;

            if (value == null && property == null)
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
