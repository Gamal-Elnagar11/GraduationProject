using AutoMapper;
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
    public class FAQController : ControllerBase
    {
         private readonly IFAQService _faqService;
         private readonly HybridCache _hybridCache;  

        public FAQController( IFAQService fAQService, HybridCache hybridCache)
        {
             _faqService = fAQService;
             _hybridCache = hybridCache;
        }



        [HttpGet("Quastions")]
        public async Task<IActionResult> GetAllQuastion(CancellationToken ct = default)
        {
            string cacheKey = "AllFAQs";
            var result = await _hybridCache.GetOrCreateAsync(
                    cacheKey,
                    async token => await _faqService.GetAllQuastion(),  
                    tags: new[] { "faqs-tag" },
                    cancellationToken: ct
                );
            return Ok(result);
        }

       
        
        [HttpGet("Answer{id}", Name = "Answer-ID")]
        public async Task<IActionResult> GetAnswerById(int id, CancellationToken ct = default)
        {
            string cacheKey = $"faq:{id}";
            var result = await _hybridCache.GetOrCreateAsync(
                    cacheKey,
                    async token => await _faqService.GetAnswerById(id),  
                    tags: new[] { "faqs-tag" },
                    cancellationToken: ct
                );
            return Ok(result);
        }

    
        
        [HttpPost(Name = "AddQ")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> AddFAQ(FAQsDTO faqDTO, CancellationToken ct = default)
        {
            var result = await _faqService.AddFAQ(faqDTO);
            await _hybridCache.RemoveByTagAsync("faqs-tag", cancellationToken: ct);  
            return Ok(result);
        }

   
        
        [HttpPut(Name = "UpdateQ")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> UpdateFAQ(FAQtest faq, CancellationToken ct = default)
        {
            var updated = await _faqService.UpdateFAQ(faq);
            await _hybridCache.RemoveByTagAsync("faqs-tag", cancellationToken: ct);  
            return Ok(updated);
        }



        [HttpDelete("{id}", Name = "DeleteQ")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> DeleteFAQ(int id, CancellationToken ct = default)
        {
            await _faqService.DeleteFAQ(id);
            await _hybridCache.RemoveByTagAsync("faqs-tag", cancellationToken: ct);  
            return NoContent();
        }
    }
}