 
namespace E_Commerce_API.Reposatory.Implementation
{
    public class ProductRepo : IProductRepo
    {
        private readonly Application _db;

        public ProductRepo(Application db)
        {
            _db = db;
        }

         
        public Task<List<Product>> GetAllProductsAsync(CancellationToken ct = default)
        {
            return _db.Products.Include(a => a.Category).ToListAsync(ct);
        }


        public async Task<Product> GetProductsByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.Products.FindAsync(id,ct);
        }



        public async Task<Product> AddAsync(Product product, CancellationToken ct = default)
        {
                 await _db.Products.AddAsync(product,ct);
                return product;
        }



        public void UpdateProduct(Product product )
        {
             _db.Update(product);
        }


        public void DeleteProduct(Product product )
        {
                _db.Products.Remove(product);
              
        }


        
    }
}
