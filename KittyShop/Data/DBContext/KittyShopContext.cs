using KittyShop.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace KittyShop.Data.DBContext
{
	public class KittyShopContext : DbContext
	{
		public DbSet<User> Users { get; set; } = null!;    //da se iskljuci warning
		public DbSet<Product> Products { get; set; } = null!;
		public DbSet<ShoppingKart> ShoppingKarts { get; set; } = null!;

		//preko construktora metod za dodavanje za conn string, => u programu
		public KittyShopContext(DbContextOptions<KittyShopContext> options)
			: base(options)
		{

		}
	}
}
