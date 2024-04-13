using System.ComponentModel.DataAnnotations.Schema;

namespace KittyShop.Data.Entities
{
	public class ShoppingCart
	{
		public int ShoppingCartId { get; set; }

		public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

		[ForeignKey("UserId")]
		public int UserId { get; set; }
	}
}
