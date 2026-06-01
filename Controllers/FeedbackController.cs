
using Microsoft.AspNetCore.RateLimiting;

namespace E_Commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "UserOrAdmin")]
    [EnableRateLimiting("CountRequest")]

    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
         public FeedbackController(IFeedbackService feedbackService, IHttpContextAccessor contextAccessor)
        {
            _feedbackService = feedbackService;
         }
 


        [HttpPost(Name ="AddFB")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "UserOrAdmin")]
        public async Task<IActionResult> AddFeedback(FeedDTO feeddto)
        {
                 await _feedbackService.AddFeedback(feeddto);
                return Ok("Feedback Added Successfuly");
        }



        [HttpGet(Name ="GetFB")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> GetAllFeedback()
        {
            var result = await _feedbackService.GetAllFeedback();
            return Ok(result);
        }


        [HttpDelete("{id}",Name ="DeleteFB")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> DeleteFeedback(int id)
        {
                await _feedbackService.DeleteFeedback(id);
                return NoContent();
        }

    }
}
