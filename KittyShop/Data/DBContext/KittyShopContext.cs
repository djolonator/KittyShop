using KittyShop.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace KittyShop.Data.DBContext
{
	public class KittyShopContext : DbContext
	{
		public DbSet<User> Users { get; set; } = null!;    
		public DbSet<Product> Products { get; set; } = null!;
		public DbSet<ShoppingCart> ShoppingCarts { get; set; } = null!;
        public DbSet<CartItem> CartItems { get; set; } = null!;

     
        public KittyShopContext(DbContextOptions<KittyShopContext> options)
			: base(options)
		{

		}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}
