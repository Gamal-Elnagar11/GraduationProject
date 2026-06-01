using Microsoft.AspNetCore.Diagnostics;

namespace E_Commerce_API.ErrorHandling
{
    public class GlobalErrorHandling(IProblemDetailsService problemDetailsService) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var (StatusCode, title) = exception switch
            {
                ValidationException => (StatusCodes.Status400BadRequest, "Bad Request"),
                ArgumentException => (StatusCodes.Status400BadRequest, "Bad Request"),
                FormatException => (StatusCodes.Status400BadRequest, "Invalid Data Format"),
                KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource Not Found"),
                ForbiddenException => (StatusCodes.Status403Forbidden, "Forbidden Access "),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorize Access"),
                ConflictException => (StatusCodes.Status409Conflict, "Resource Conflict"),
                _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
            };
            // بدل ما نستخدم الـ problemDetailsService اللي ساعات بيحصل معاه تضارب في الترتيب
            // اكتب الـ JSON صراحة في الـ Response ورجع true
            httpContext.Response.StatusCode = StatusCode;
            httpContext.Response.ContentType = "application/problem+json";

            var problemDetails = new ProblemDetails
            {
                Status = StatusCode,
                Title = title,
                Detail = exception.Message,
                Type = exception.GetType().Name,
                Instance = httpContext.Request.Path
            };

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true; // كده بنقول للسيستم: خلاص أنا هندلت الأكسبشن ده ومحدش يلمسه بعدي
        }
    }
}
