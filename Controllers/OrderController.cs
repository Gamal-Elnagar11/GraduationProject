

using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.Hybrid;

namespace E_Commerce_API.Controllers
{ 
        [Route("api/[controller]")]
        [ApiController]
       [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "UserOrAdmin")]
    [EnableRateLimiting("CountRequest")]

    public class OrdersController : ControllerBase
        {
            private readonly IOrderService _orderService;
              private readonly HybridCache _hybridCache;



        public OrdersController(IOrderService orderService, HybridCache hybridCache)
        {
            _orderService = orderService;
             _hybridCache = hybridCache;
        }

        [HttpPost("Checkout->Create-Order")]
        [Consumes("application/json")]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
         [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        [EndpointName("Gamal1")]
        [EndpointSummary("Create New Order")]
        [EndpointDescription("Create New Order and Confirm Shippeing Data")]

            public async Task<IActionResult> Checkout([FromBody] CheckoutDTO checkDTO, CancellationToken ct = default)
            { 
                    var order = await _orderService.Checkout(checkDTO,ct);
                     await _hybridCache.RemoveByTagAsync("products-tag", cancellationToken: ct);
                     return Ok(order);
            }


        [HttpGet("My-Orders")]
        [Consumes("application/json")]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status200OK)]
         [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        [EndpointName("Gamal2")]
        [EndpointSummary("Get My Order")]
        [EndpointDescription("Get My Order From System and Get Status Of Order")]
         
            public async Task<IActionResult> GetMyOrders(CancellationToken ct = default)
            {
                   var orders = await _orderService.GetOrdersByUser(ct);
                     return Ok(orders);
            }




        [HttpGet("{orderId}")]
        [Consumes("application/json")]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status200OK)]
         [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        [EndpointName("Gamal3")]
        [EndpointSummary("Get Order By Id")]
        [EndpointDescription("Get Any Cart With Id")]

         public async Task<IActionResult> GetOrderById(int orderId, CancellationToken ct = default)
            {
                    var order = await _orderService.GetOrderById(orderId,ct);
                     return Ok(order);
            }

         
        [HttpGet("AllOrders")]
        [Consumes("application/json")]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status200OK)]
         [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        [EndpointName("Gamal4")]
        [EndpointSummary("Get All Orders")]
        [EndpointDescription("Get All Orders From System")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders(CancellationToken ct = default)
            {
                    var orders = await _orderService.GetAllOrders(ct);
                     return Ok(orders);
            }
         
        
        
        [HttpPut("{orderId}/StatusOrder")]
        [Consumes("application/json")]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status200OK)]
         [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        [EndpointName("Gamal5")]
        [EndpointSummary("Update Status Order")]
        [EndpointDescription("Controll Of Status Order Like Peinding or Confirmed")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromQuery] OrderStatus status)
            {
                     await _orderService.UpdateOrderStatus(orderId, status);
                     return NoContent();
            }


        [HttpGet("Payment-Methods")]
        [Consumes("application/json")]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status200OK)]
         [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
         [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        [EndpointName("Gamal6")]
        [EndpointSummary("Payment Methods")]
        [EndpointDescription("Get All Payment Method From System")]

        public IActionResult GetPaymentMethods()
            {
                var methods = Enum.GetValues(typeof(Payment))
                                  .Cast<Payment>()
                                  .Select(m => m.ToString())
                                  .ToList();
                return Ok(methods);
            }




        [HttpDelete("{id}")]
          [ProducesResponseType<ProblemDetails>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
       // [EndpointName("Gamal6")]
        [EndpointSummary("Delete Order By ID")]
        [EndpointDescription("Delete Order By ID From System")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

        public async Task<IActionResult> DeleteOrderByIDD(int id)
        {
             
                await  _orderService.DeleteOrderByID(id);
                return Ok("Order Deleted Successfuly");
             
        }


    
    }
    
}
