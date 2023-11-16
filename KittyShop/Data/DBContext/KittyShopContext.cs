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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    UserName = "Admin",
                    Password = "CfDJ8KcykMeAMZBMluQ8scxwOHwyWT2d4QJJYH2zz8RR6hvE-jYEU6eHZFG23A3_se2Bc4NqlGHnGFX2zc2KZqg5X9y58xi54-KepJz6byAzbUhHwm_mxUKIVqCz1_JFIfpPlg",
                    Type = Enums.UserTypes.Admin
                }
            );
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 2,
                    UserName = "Customer",
                    Password = "CfDJ8KcykMeAMZBMluQ8scxwOHzaSo9UJCszqlDHBvu76FDI5ad4W9wtq58bv0AOaV2Oi9wqjTqxXlC7cbqp_R6lRbQW4jIBXAJLZvim8oATkspShiz1cV4x2kD20EiKYak9Jg",
                    Type = Enums.UserTypes.Regular
                }
            );
        }
    }
}
