 
namespace E_Commerce_API.Service.Interface
{
    public interface ICategoryService
    {
        public Task<List<CategoriesDTO>> GetAllCateory(CancellationToken ct = default);
        public Task <List<CategorywithProductDTO>> GetAllCateoryWithProducts(CancellationToken ct = default);
        public Task<CategorywithProductDTO> GetCategoryByIdAsync(int id, CancellationToken ct = default);
        public Task<CategoriesDTO> AddCategoryAsync(CategoryName category);
        public Task<CategoryName> UpdateCategoryAsync(int id ,CategoryName category);
        public Task DeleteCategoryAsync(int id);
        public Task<List<CategoriesDTO>>  Search (string name, CancellationToken ct = default);
        public Task<List<CategorywithProductDTO>> SearchWithProducts(string name, CancellationToken ct = default);  
    }
}
