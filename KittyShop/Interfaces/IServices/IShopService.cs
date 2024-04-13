namespace KittyShop.Interfaces.IServices
{
    public interface IShopService
    {
        Task CreateCartForUserAsync(int userId);
        Task<(bool isAdded, string message)> AddProductToCartAsync(int userId, int productId);
    }
}
