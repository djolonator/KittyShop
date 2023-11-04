using System.ComponentModel.DataAnnotations;

namespace KittyShop.Models
{
    public class EditProfileModel
    {
        public int UserId { get; set; }
        
        public string UserName { get; set; } = string.Empty;
       
        public string Password { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? NewUserName { get; set; } = string.Empty;

        public string? NewPassword { get; set; } = string.Empty;

        [Compare("NewPassword", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string? NewConfirmPassword { get; set; } = string.Empty;

        public string? NewEmail { get; set; }
    }
}
