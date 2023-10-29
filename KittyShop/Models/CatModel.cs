using KittyShop.Services.Utility;
using System.ComponentModel.DataAnnotations;

namespace KittyShop.Models
{
    public class CatModel
    {
        public int ProductId { get; set; }

        [Required]
        public string Race { get; set; } = null!;

        [Required]
        public string FurrColor { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public string EyesColor { get; set; } = null!;

        [Required]
        public float Price { get; set; }

        public string? ImgUrlPath { get; set; }

        [ValidationForImage("ImgUrlPath")]
        public IFormFile? Image { get; set; }
    }
}
