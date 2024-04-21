
namespace KittyShop.Models
{
    public class ShoppingCartModel
    {
        public int ShoppingCartId { get; set; } 
        public List<CartItemModel> CartItems { get; set; }
        public float TotalPrice { get; set; }
        public int UserId { get; set; }
    }
}
