using KittyShop.Enums;
using System.ComponentModel.DataAnnotations;

namespace KittyShop.Models
{
	public class RegisterModel
	{
		[Required]
		public string UserName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string? Email { get; set; }
    }
}
