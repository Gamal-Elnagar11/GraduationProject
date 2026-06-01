 
namespace E_Commerce_API.Service.Implementation
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<CategoryService> logger;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.logger = logger;
        }



        public async Task<CategoriesDTO> AddCategoryAsync(CategoryName category)
        {
             await _unitOfWork.BeginTransactionAsync();

            try
            {
                 var map = mapper.Map<Category>(category);
                 await _unitOfWork.Repositoey<Category>().AddAsync(map);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommmetTransactionAsync();
                var result = mapper.Map<CategoriesDTO>(map);
                logger.LogInformation("Add Category successfuly");

                return result;
            }
            catch (Exception)
            {
                await _unitOfWork.RollebackAsync();
                throw;
            }
        }


        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _unitOfWork.CategoryRepo.GeCategoriesByIdAsync(id);
            if (category == null)
                throw new ArgumentException("Category ID Not Found");

            if (category.Products.Any())
                throw new ArgumentException("This Category has Products");

             // _unitOfWork.CategoryRepo.DeleteCategory(category);   // alrady delete from DB

              category.IsDeleted = true;               // Soft Delete 
            logger.LogInformation("Delete Category Successfuly ID {ID}. At {Time}", id, DateTime.Now);
              await _unitOfWork.CompleteAsync();
 
        }

        
         
        public  async Task<List<CategoriesDTO>> GetAllCateory(CancellationToken ct = default)
        {
             var result = await _unitOfWork.CategoryRepo.GetAllCategoriesAsync(ct);
            var map = mapper.Map<List<CategoriesDTO>>(result);
            return map;
         }

        public async Task<List<CategorywithProductDTO>> GetAllCateoryWithProducts(CancellationToken ct = default)
        {
            var result = await _unitOfWork.CategoryRepo.GetAllCategoriesWithProducts(ct);
            var map = mapper.Map<List<CategorywithProductDTO>>(result);
            return map;
        }

        public async Task<CategorywithProductDTO> GetCategoryByIdAsync(int id, CancellationToken ct = default)
        {
            var result = await _unitOfWork.CategoryRepo.GeCategoriesByIdAsync(id,ct);
            if (result == null)
                throw new ArgumentException("Category ID Not Found");
            if (result.IsDeleted == true)
                throw new ArgumentException("This Category was Deleted");
            var map = mapper.Map<CategorywithProductDTO>(result);
            return map;
        }

        public async Task<List<CategoriesDTO>> Search(string name, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Search value is required");
            var result =  await _unitOfWork.CategoryRepo.Search(name,ct);
            var map = mapper.Map<List<CategoriesDTO>>(result);
            return map;
        }

        public async Task<List<CategorywithProductDTO>> SearchWithProducts(string name, CancellationToken ct = default)
        {
            var result = await _unitOfWork.CategoryRepo.SearchwithProducts(name,ct);
            var map = mapper.Map<List<CategorywithProductDTO>>(result);
            return map;
        }

        public async Task<CategoryName> UpdateCategoryAsync(int id, CategoryName category)
        {
            var findid = await _unitOfWork.CategoryRepo.GeCategoriesByIdAsync(id);
            if (findid == null)
                throw new ArgumentException("Category ID Not Found");

            findid.Name = category.Name;
            

              _unitOfWork.CategoryRepo.UpdateCategory(findid);
             await _unitOfWork.CompleteAsync();
            var map = mapper.Map<CategoryName>(findid);
            logger.LogInformation("Update Category Successfuly ID {ID}. At {Time}.", id, DateTime.Now);
            return map;

        }

       
    }
}
