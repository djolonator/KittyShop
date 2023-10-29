using KittyShop.Data.DBContext;
using KittyShop.Data.Entities;
using KittyShop.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

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
