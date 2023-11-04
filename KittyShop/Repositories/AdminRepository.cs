using KittyShop.Data.DBContext;
using KittyShop.Data.Entities;
using KittyShop.Interfaces.IRepositories;


namespace KittyShop.Repositories
{
    public class AdminRepository: IAdminRepository
    {
        private readonly KittyShopContext _context;
        public AdminRepository(KittyShopContext context) 
        {
            _context = context;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

        public async Task AddProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await SaveChangesAsync();
        }

        public async Task<Product?> FindProductByIdAsync(int productId)
        {
            return await _context.Products.FindAsync(productId);
        }
    }
}
