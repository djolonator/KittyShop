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

            products = AddToQueryFurrFilter(furrColor, products);
            products = AddToQueryEyesFilter(eyesColor, products);
            products = AddToQueryDescriptionFilter(description, products);
            products = AddToQueryRaceFilter(race, products);

            var paginatedProductsModel = ConvertEntityToModel(products);
            var listToReturn = await PaginatedList<CatModel>.CreateAsync(paginatedProductsModel.AsNoTracking(), pageNumber ?? 1, pageSize);

            return listToReturn;
        }

        private IQueryable<CatModel> ConvertEntityToModel(IQueryable<Product> products)
        {
            return products
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
        }

        private IQueryable<Product> AddToQueryFurrFilter(string furrColor, IQueryable<Product> products)
        {
            if (furrColor != null) 
            {
                furrColor = furrColor.Trim();
                products = products.Where(p => p.FurrColor.Contains(furrColor));
            }
            return products;
        }

        private IQueryable<Product> AddToQueryEyesFilter(string eyesColor, IQueryable<Product> products)
        {
            if (eyesColor != null)
            {
                eyesColor = eyesColor.Trim();
                products = products.Where(p => p.EyesColor.Contains(eyesColor));
            }
            return products;
        }

        private IQueryable<Product> AddToQueryDescriptionFilter(string description, IQueryable<Product> products)
        {
            if (description != null)
            {
                description = description.Trim();
                products = products.Where(p => p.Description.Contains(description));
            }
            return products;
        }

        private IQueryable<Product> AddToQueryRaceFilter(string race, IQueryable<Product> products)
        {
            if (race != null)
            {
                race = race.Trim();
                products = products.Where(p => p.Race.Contains(race));
            }
            return products;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            return await SaveChangesAsync();
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
