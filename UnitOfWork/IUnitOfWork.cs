
namespace E_Commerce_API.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
       public  IGenaricRepo<T> Repositoey<T>() where T : class;

       public  ICategoryRepo CategoryRepo { get; }
       public  IProductRepo ProductRepo { get; }
       public  ICartRepo CartRepo { get; }
        public IOrderRepo OrderRepo { get; }

       // Task<Cart> GetCartByUserIdAsync(string userId); 
         
        public Task<IDbContextTransaction> BeginTransactionAsync ();
        public Task CommmetTransactionAsync ();
        public Task RollebackAsync ();
        Task<int> CompleteAsync();
    }
}
