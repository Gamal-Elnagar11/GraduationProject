 
namespace E_Commerce_API.Reposatory.Interface
{
    public interface IGetUserCart
    {
        public Task<Cart> GetUserIdInCartAsync(string userid);
        
    }
}
