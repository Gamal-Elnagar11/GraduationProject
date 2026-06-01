 
namespace E_Commerce_API.Reposatory.Interface
{
    public interface ICartRepo
    {
        public Task<Cart> GetCartByUserId(string userId, CancellationToken ct = default);
        public Task<List<Cart>> GetAllCarts();
        public Task<Cart> AddCart(Cart cart);
        public void DeleteCart(Cart cart);
        public void DeleteCartItems(IEnumerable<CartItem> cartItems);



    }
}
