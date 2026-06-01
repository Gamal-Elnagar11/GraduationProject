 
namespace E_Commerce_API.Reposatory.Interface
{
    public interface IProductRepo
    {
        public Task<List<Product>> GetAllProductsAsync(CancellationToken ct = default);
       public Task<Product> GetProductsByIdAsync(int id, CancellationToken ct = default);
       public Task<Product> AddAsync(Product product, CancellationToken ct = default);
       public void  UpdateProduct(Product product);
       public void DeleteProduct(Product product );

    }
}
