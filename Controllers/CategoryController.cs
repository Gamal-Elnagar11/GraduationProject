using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.Hybrid;

namespace E_Commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("CountRequest")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly HybridCache _hybridCache;

        public CategoryController(ICategoryService categoryService, HybridCache hybridCache)
        {
            _categoryService = categoryService;
            _hybridCache = hybridCache;
        }

        [HttpGet("{id}", Name = "GetById")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
        {
             string cacheKey = $"category:{id}";
            var result = await _hybridCache.GetOrCreateAsync(cacheKey,
                async token => await _categoryService.GetCategoryByIdAsync(id, token),
                tags: new[] { "categories-tag" }, cancellationToken: ct);
            return Ok(result);
        }

        
        
        [HttpGet("Get All Category With Products")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllWithProducts(CancellationToken ct = default)
        {
             string cacheKey = "CategoriesWithProducts";
            var result = await _hybridCache.GetOrCreateAsync(cacheKey,
                async token => await _categoryService.GetAllCateoryWithProducts(token),
                tags: new[] { "categories-tag" }, cancellationToken: ct);
            return Ok(result);
        }

      
        
        [HttpGet("Get All Category")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCategory(CancellationToken ct = default)
        {
             string cacheKey = "AllCategories";
            var allcategory = await _hybridCache.GetOrCreateAsync(cacheKey,
                async token => await _categoryService.GetAllCateory(token),
                tags: new[] { "categories-tag" }, cancellationToken: ct);

            return Ok(allcategory);
        }

       
        
        [HttpGet("Search Category")]
        [AllowAnonymous]
        public async Task<IActionResult> Search(string? name, CancellationToken ct = default)
        {
             string cacheKey = $"search:categories:{name ?? "all"}";
            var categories = await _hybridCache.GetOrCreateAsync(cacheKey,
                async token => await _categoryService.Search(name, token),
                tags: new[] { "categories-tag" }, cancellationToken: ct);

            return Ok(categories);
        }

       
        
        [HttpGet("Search With Products")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchWithProducts(string? name, CancellationToken ct = default)
        {
             string cacheKey = $"search:categories-products:{name ?? "all"}";
            var categories = await _hybridCache.GetOrCreateAsync(cacheKey,
                async token => await _categoryService.SearchWithProducts(name, token),
                tags: new[] { "categories-tag" }, cancellationToken: ct);

            return Ok(categories);
        }

     
        
        [HttpPost("AddCategory")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> AddCategory(CategoryName categoriesDTO, CancellationToken ct = default)
        {
            var category = await _categoryService.AddCategoryAsync(categoriesDTO);
            await _hybridCache.RemoveByTagAsync("categories-tag", cancellationToken: ct);
            return Ok(category);
        }

     
        
        [HttpPut("{id}", Name = "UpdateCategory")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> UpdatCategory(int id, CategoryName categoryName, CancellationToken ct = default)
        {
            var result = await _categoryService.UpdateCategoryAsync(id, categoryName);
            await _hybridCache.RemoveByTagAsync("categories-tag", cancellationToken: ct);
            return Ok(result);
        }

      
        
        [HttpDelete("{id}", Name = "DeleteCategory")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id, CancellationToken ct = default)
        {
            await _categoryService.DeleteCategoryAsync(id);
            await _hybridCache.RemoveByTagAsync("categories-tag", cancellationToken: ct);
            return NoContent();
        }
    }
}