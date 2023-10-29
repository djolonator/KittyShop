using KittyShop.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KittyShop.Data.Entities
{
	public class User
	{
		[Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
		public string UserName { get; set; } = null!;
		public string Password { get; set; } = null!;
		public string? Email { get; set; }
		public UserTypes Type { get; set; }
	}
}
