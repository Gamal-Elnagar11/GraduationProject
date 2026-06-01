 
namespace E_Commerce_API.Service.Interface
{
    public interface IProductService
    {
       public Task<List<ProductBaseDTO>> GetAllProductsAsync(CancellationToken ct = default );
       public Task<ProductBaseDTO> GetProductByIdAsync(int id, CancellationToken cancellationToken = default);
       public Task<ProductBaseDTO> UpdateProductAsync(int id , UpdateProductDTO product, CancellationToken cancellationToken = default);
        public Task<ProductBaseDTO> UpdateStock(int id ,int stock, CancellationToken cancellationToken = default);
       public Task<ProductBaseDTO> AddProductAsync(AddProductDTO product, CancellationToken cancellationToken = default);
       public Task DeleteProductAsync(int id, CancellationToken ct = default);
        public Task<List<ProductBaseDTO>> Search(string name, CancellationToken cancellationToken = default);

        public Task<bool> CategoryExistsAsync(int categoryId, CancellationToken cancellationToken = default);

       


    }
}
