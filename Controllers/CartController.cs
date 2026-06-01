

using Microsoft.AspNetCore.RateLimiting;

namespace E_Commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme ,Policy = "UserOrAdmin")]
    [EnableRateLimiting("CountRequest")]

    public class CartController : ControllerBase
    {
          private readonly ICartService _cartService;
 
            public CartController(ICartService cartService)
            {
                _cartService = cartService;
             }

        [HttpGet("MyCart")]
        public async Task<IActionResult> GetMyCart(CancellationToken ct = default)
        {
            // التعديل: بننادي الميثود اللي بترجع الـ DTO المتظبط والمحمي من الـ Object Cycle
            var cartDto = await _cartService.GetCartByUserId(ct);
            return Ok(cartDto);
        }

        [HttpPost("add-item-to-cart")]
        public async Task<IActionResult> AddItemToCart(int productId, int quantity)
        {
            await _cartService.AddItemCart(productId, quantity);
            return NoContent();
        }

        [HttpPut("update-item-in-cart")]
        public async Task<IActionResult> UpdateItemQuantity(int productId, int newQuantity)
        {
            await _cartService.UpdateItemCartQuantity(productId, newQuantity);
            return NoContent();
        }

        [HttpDelete("remove-item-from-cart")]
        public async Task<IActionResult> RemoveItemFromCart(int productId)
        {
            await _cartService.DeleteItemFromCart(productId);
            return NoContent();
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            // بنجيب الـ Entity عشان ميثود الـ ClearCart مستنياه كباراميتر
            var cart = await _cartService.GetOrCreateCart();
            await _cartService.ClearCart(cart);
            return NoContent();
        }

    }
}

