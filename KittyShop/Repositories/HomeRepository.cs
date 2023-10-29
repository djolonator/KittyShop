using KittyShop.Data.DBContext;
using KittyShop.Data.Entities;
using KittyShop.Interfaces.IRepositories;
using KittyShop.Models;
using KittyShop.Services.Utility;
using Microsoft.EntityFrameworkCore;

namespace KittyShop.Repositories
{
    public class HomeRepository: IHomeRepository
    {
        private readonly KittyShopContext _context;
        public HomeRepository(KittyShopContext context) 
        {
            _context = context;
        }


        public async Task<PaginatedList<CatModel>> GetProducts(string furrColor, string eyesColor,
            string description, string race, int? pageNumber, int pageSize)
        {
            var products = _context.Products as IQueryable<Product>;

            if (!string.IsNullOrEmpty(furrColor))
            {
                furrColor = furrColor.Trim();
                products = products.Where(p => p.FurrColor.Contains(furrColor));
            }

            if (!string.IsNullOrEmpty(eyesColor))
            {
                eyesColor = eyesColor.Trim();
                products = products.Where(p => p.EyesColor.Contains(eyesColor));
            }

            if (!string.IsNullOrEmpty(description))
            {
                description = description.Trim();
                products = products.Where(p => p.Description.Contains(description));
            }

            if (!string.IsNullOrEmpty(race))
            {
                race = race.Trim();
                products = products.Where(p => p.Race.Contains(race));
            }


            var paginatedProductsModel = products
                .Select(p => new CatModel
                {
                    ProductId = p.ProductId,
                    Race = p.Race,
                    FurrColor = p.FurrColor,
                    Description = p.Description,
                    Price = p.Price,
                    EyesColor = p.EyesColor,
                    ImgUrlPath = p.ImgUrlPath
                }).AsQueryable();


            var listToReturn = await PaginatedList<CatModel>.CreateAsync(paginatedProductsModel.AsNoTracking(), pageNumber ?? 1, pageSize);
            return listToReturn;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

        public async Task CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            var id = await SaveChangesAsync();
        }

        public async Task<bool> UserNameExistsAsync(string userName)
        {
            return await _context.Users.AnyAsync(x => x.UserName == userName);
        }

        public async Task<User?> FindUserByIdAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<User?> FindUserByNameAsync(string userName)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.UserName == userName);
        }
    }
}
