using System.ComponentModel.DataAnnotations;


namespace KittyShop.Utility
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
            var imgPath = GetPathOfCurrentImageForProduct(validationContext);

            if (IsFileUploaded(value))
            {
                if (!ValidateImageIsRightType(value!))
                    return new ValidationResult("Image must be jpeg format");
                if (!ValidateImageIsRightSize(value!))
                    return new ValidationResult("Maximum size of image is one megabyte");
                if (!ValidateImageIsOneToOneRatio(value!))
                    return new ValidationResult("Ratio of image must be 1:1");
            }
            else if (!ProductHasImage(imgPath))
                return new ValidationResult("You must upload image for product");

            return ValidationResult.Success!;

        }
        private string? GetPathOfCurrentImageForProduct(ValidationContext validationContext)
        {
            var imgPath = string.Empty;

            var prop = validationContext.ObjectInstance.GetType().GetProperty(_imgUrlPath);
            var imgPathProperty = prop!.GetValue(validationContext.ObjectInstance);

            if (imgPathProperty != null)
            {
                imgPath = imgPathProperty.ToString();
            }

            return imgPath;
        }

        private bool IsFileUploaded(object? value)
        {
            return value is null ? false : true;
        }
        private bool ProductHasImage(string? imgPath)
        {
            return string.IsNullOrEmpty(imgPath) ? false : true;
        }

        private bool ValidateImageIsRightType(object value)
        {
            bool vedrict = false;
            var contentTypeProp = value!.GetType().GetProperty("ContentType");

            if (contentTypeProp != null)
            {
                var contentTypeValue = contentTypeProp.GetValue(value)?.ToString();

                if (contentTypeValue != null && contentTypeValue.Contains("jpeg", StringComparison.OrdinalIgnoreCase))
                    vedrict = true;
            }

            return vedrict;
        }

        private bool ValidateImageIsRightSize(object value)
        {
            bool vedrict = false;
            var lengthTypeProp = value!.GetType().GetProperty("Length");

            if (lengthTypeProp != null)
            {
                var lengthTypeValue = lengthTypeProp.GetValue(value)?.ToString();

                if (lengthTypeValue != null && int.Parse(lengthTypeValue) < 995611) 
                    vedrict = true;
            }

            return vedrict;
        }

        private bool ValidateImageIsOneToOneRatio(object value)
        {
            int width = 0;
            int height = 0;
            var file = value as IFormFile;

            if (file != null)
            {
                using (var image = Image.Load(file.OpenReadStream()))
                {
                    width = image.Width;
                    height = image.Height;
                }
            }

            return width == height ? true : false;
        }
    }
}
