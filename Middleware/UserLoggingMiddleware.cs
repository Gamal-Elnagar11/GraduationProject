using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System.Security.Claims;
using System.Threading.Tasks;

public class UserLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public UserLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string username = "Anonymous";

        // نـتأكد الأول إن اليوزر معموله Authentication والـ Claims موجودة
        if (context.User.Identity?.IsAuthenticated == true)
        {
            // 🎯 الحركة الصايعة: بندور على أي Claim فيه اسم أو إيميل أو معرّف
            username = context.User.FindFirst(ClaimTypes.Name)?.Value
                       ?? context.User.FindFirst("username")?.Value
                       ?? context.User.FindFirst(ClaimTypes.Email)?.Value
                       ?? context.User.FindFirst("email")?.Value
                       ?? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? context.User.Identity.Name
                       ?? "Authenticated User"; // لو ملحقناش اسم خالص بس هو عامل login
        }

        // تثبيت الاسم في الـ LogContext
        using (LogContext.PushProperty("UserName", username))
        {
            await _next(context);
        }
    }
}