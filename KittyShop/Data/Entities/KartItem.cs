using System.ComponentModel.DataAnnotations.Schema;

namespace KittyShop.Data.Entities
{
	public class KartItem
	{
		public int KartItemId { get; set; }

		[ForeignKey("ShoppingKartId")]
		public ShoppingKart ShoppingKart { get; set; } = null!;

		[ForeignKey("ProductId")]
		public Product Product { get; set; } = null!;
	}
}
