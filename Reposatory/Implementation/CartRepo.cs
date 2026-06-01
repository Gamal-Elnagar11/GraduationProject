 

namespace E_Commerce_API.Reposatory.Implementation
{
    public class CartRepo : ICartRepo
    {

        private readonly Application _db;

        public CartRepo(Application db)
        {
            _db = db;
        }

        public async Task<Cart> AddCart(Cart cart)
        {
             await _db.Carts.AddAsync(cart);
            return cart;
         }

        public  void DeleteCart(Cart cart)
        {
           _db.Carts.Remove(cart);   
        }
        public void DeleteCartItems(IEnumerable<CartItem> cartItems)
        {
            _db.CartItems.RemoveRange(cartItems);
        }
        public async Task<List<Cart>> GetAllCarts()
        {
            var result = await _db.Carts.Include(a => a.CartItems)
                      .ThenInclude(a => a.Products).ToListAsync();
            return result;
        }

        public  async Task<Cart> GetCartByUserId(string userId, CancellationToken ct = default)
        {
            return await _db.Carts.Include(a => a.CartItems)
                         .ThenInclude(a => a.Products)
                         .FirstOrDefaultAsync(a => a.UserId == userId,ct);
                         

         }
    }
}
