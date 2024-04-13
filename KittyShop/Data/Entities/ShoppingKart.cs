using System.ComponentModel.DataAnnotations.Schema;

namespace KittyShop.Data.Entities
{
	public class ShoppingKart
	{
		public int ShoppingKartId { get; set; }

		public ICollection<KartItem> KartItems { get; set; } = new List<KartItem>();

		[ForeignKey("UserId")]
		public User User { get; set; }
	}
}
