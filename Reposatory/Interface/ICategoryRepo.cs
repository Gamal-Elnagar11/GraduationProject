 
namespace E_Commerce_API.Reposatory.Interface
{
    public interface ICategoryRepo
    {
        public Task<List<Category>> GetAllCategoriesAsync(CancellationToken ct = default);
        public Task<List<Category>> GetAllCategoriesWithProducts(CancellationToken ct = default);
        public Task<Category> GeCategoriesByIdAsync(int id, CancellationToken ct = default);
        public Task<Category> AddAsync(Category category);
        public void UpdateCategory(Category category);
        public void DeleteCategory(Category category);
        public Task<List<Category>> Search(string name, CancellationToken ct = default);
        public Task<List<Category>> SearchwithProducts(string name, CancellationToken ct = default);
    }
}
