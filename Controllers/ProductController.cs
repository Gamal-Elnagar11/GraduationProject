using FluentValidation;
using Humanizer;
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
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IValidator<AddProductDTO> _addProductValidator;
        private readonly HybridCache _hybridCache;

        public ProductController(IProductService productService, IValidator<AddProductDTO> addProductValidator, HybridCache hybridCache)
        {
            _productService = productService;
            _addProductValidator = addProductValidator;
            _hybridCache = hybridCache;
        }

        [HttpGet("Products")]
        [Consumes("application/json")]
        [ProducesResponseType<List<ProductBaseDTO>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Get All Products")]
        [EndpointDescription("Get All Products From System")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProduct(CancellationToken ct = default)
        {
            string cacheKey = "AllProducts";
            var result = await _hybridCache.GetOrCreateAsync(
                    cacheKey,
                    async token => await _productService.GetAllProductsAsync(token),
                    tags: new[] { "products-tag" },  
                    cancellationToken: ct
                );
            return Ok(result);
        }

        [HttpGet("Search")]
        [Consumes("application/json")]
        [ProducesResponseType<List<ProductBaseDTO>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Search in Products")]
        [EndpointDescription("Get All Products From System That Contain Value In Search")]
        [AllowAnonymous]
        public async Task<IActionResult> Search(string? name, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Search Value is required");
            }

             string cacheKey = $"search:products:{name}";
            var result = await _hybridCache.GetOrCreateAsync(
                    cacheKey,
                    async token => await _productService.Search(name, token),
                    tags: new[] { "products-tag" },
                    cancellationToken: ct
                );

            return Ok(result);
        }

        [HttpGet("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType<ProductBaseDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Get Product By Id")]
        [EndpointDescription("Get Product From System By Id")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductById(int id, CancellationToken ct = default)
        {
             string cacheKey = $"product:{id}";
            var result = await _hybridCache.GetOrCreateAsync(
                    cacheKey,
                    async token => await _productService.GetProductByIdAsync(id, token),
                    tags: new[] { "products-tag" },
                    cancellationToken: ct
                );

            return Ok(result);
        }

        [HttpPost("AddProduct")]
        [ProducesResponseType<ProductBaseDTO>(StatusCodes.Status201Created)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Add New Product")]
        [EndpointDescription("Add a New Product To System Specific By Admin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> AddProduct([FromForm] AddProductDTO productDTO, CancellationToken ct = default)
        {
            var validationResult = await _addProductValidator.ValidateAsync(productDTO, ct);

            var result = await _productService.AddProductAsync(productDTO);

             await _hybridCache.RemoveByTagAsync("products-tag", cancellationToken: ct);

            return CreatedAtAction(nameof(GetProductById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType<ProductBaseDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Update Product By Id")]
        [EndpointDescription("Update a Product From System Specific By Admin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromForm] UpdateProductDTO updateDTO, CancellationToken ct = default)
        {
            var result = await _productService.UpdateProductAsync(id, updateDTO);

             await _hybridCache.RemoveByTagAsync("products-tag", cancellationToken: ct);

            return Ok(result);
        }

        [HttpPut("/stock/{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Update Stock By Id")]
        [EndpointDescription("Update a Stock Product From System Specific By Admin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> UpdateQuantity(int id, int stock, CancellationToken ct = default)
        {
            await _productService.UpdateStock(id, stock);

             await _hybridCache.RemoveByTagAsync("products-tag", cancellationToken: ct);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Delete Product By Id")]
        [EndpointDescription("Delete Product From System Specific By Admin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id, CancellationToken ct = default)
        {
            await _productService.DeleteProductAsync(id);

             await _hybridCache.RemoveByTagAsync("products-tag", cancellationToken: ct);

            return NoContent();
        }
    }
}