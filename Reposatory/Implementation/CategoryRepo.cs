 
namespace E_Commerce_API.Reposatory.Implementation
{
    public class CategoryRepo : ICategoryRepo
    {
        private readonly Application _db;

        public CategoryRepo(Application db)
        {
            _db = db;
        }

        public async Task<Category> AddAsync(Category category)
        {
              await _db.Categories.AddAsync(category);
            return category;
        }

        public void DeleteCategory(Category product)
        {
            _db.Categories.Remove(product);
        }

        public async Task<Category> GeCategoriesByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.Categories.Include(a => a.Products).FirstOrDefaultAsync( a=> a.Id == id,ct);
        }
        public async Task<List<Category>> GetAllCategoriesAsync(CancellationToken ct = default)
        {
             return await _db.Categories
                            .Where(c => !c.IsDeleted)
                            .AsNoTracking()
                            .ToListAsync(ct);
        }

        public async Task<List<Category>> GetAllCategoriesWithProducts(CancellationToken ct = default)
        {
             return await _db.Categories
                            .Where(c => !c.IsDeleted)
                            .Include(c => c.Products)
                            .AsNoTracking()
                            .ToListAsync(ct);
        }

        public async Task<List<Category>> Search(string name, CancellationToken ct = default)
        {
             return await _db.Categories
                            .Where(c => !c.IsDeleted && c.Name.Contains(name))
                            .AsNoTracking()
                            .ToListAsync(ct);
        }

        public async Task<List<Category>> SearchwithProducts(string name, CancellationToken ct = default)
        {
             return await _db.Categories
                            .Where(c => !c.IsDeleted && c.Name.Contains(name))
                            .Include(a => a.Products)
                            .AsNoTracking()
                            .ToListAsync(ct);
        }

        public void UpdateCategory(Category product)
        {
               
            _db.Update(product);
        }
    }
} 