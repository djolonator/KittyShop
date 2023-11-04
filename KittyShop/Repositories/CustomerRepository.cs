using KittyShop.Data.DBContext;
using KittyShop.Interfaces.IRepositories;

namespace KittyShop.Repositories
{
    public class CustomerRepository: ICustomerRepository
    {
        private readonly KittyShopContext _context;
       public CustomerRepository(KittyShopContext context)
       {
            _context = context;
       }
    }
}
