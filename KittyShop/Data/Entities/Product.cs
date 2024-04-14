namespace KittyShop.Data.Entities
{
	public class Product
	{
		public int ProductId { get; set; }
		public string Race { get; set; } = null!;
		public string FurrColor { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string EyesColor { get; set; } = null!;
		public float Price { get; set; }
        public string ImgUrlPath { get; set; } = null!;
	}
}
