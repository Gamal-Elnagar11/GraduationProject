 
namespace E_Commerce_API.Service.Interface
{
    public interface ICartService
    {
        public Task<ResponseCartDTO> GetCartByUserId(CancellationToken ct = default);
 
        public Task<Cart> GetOrCreateCart(CancellationToken ct = default);
         public Task ClearCart(Cart cart);


        public Task AddItemCart( int productid, int quantity);
        public Task UpdateItemCartQuantity(int productid, int newquantity);
        public Task DeleteItemFromCart( int productid);
    }
}
