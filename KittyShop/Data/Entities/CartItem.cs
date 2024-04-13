using System.ComponentModel.DataAnnotations.Schema;

namespace KittyShop.Data.Entities
{
	public class CartItem
	{
		public int CartItemId { get; set; }

		[ForeignKey("ShoppingCartId")]
		public int ShoppingCartId { get; set; }

		[ForeignKey("ProductId")]
		public int ProductId { get; set; }
	}
}
