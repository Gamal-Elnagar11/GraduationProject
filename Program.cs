
using E_Commerce_API.DependencyInjection;
using E_Commerce_API.ErrorHandling;
 using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.JSInterop;
using Mscc.GenerativeAI;
using Mscc.GenerativeAI.Types;
using Serilog;
using System.Reflection;

namespace E_Commerce_API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
           JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            var builder = WebApplication.CreateBuilder(args);
            
  
            

           var context1 = builder.Services.AddDbContext<Application>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("conn")));
             



              builder.Services.DIService(builder.Configuration);    // Add DI

            builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            builder.Services.AddFluentValidationAutoValidation();

            // Chaching
            builder.Services.AddHybridCache(options =>
            {
                options.DefaultEntryOptions = new HybridCacheEntryOptions
                {
                     Expiration = TimeSpan.FromDays(5),
                    LocalCacheExpiration = TimeSpan.FromDays(5)
                };
            });

            builder.Host.UseSerilog((context, configuration) =>
                configuration
                    .ReadFrom.Configuration(context.Configuration) // بيقرأ الإعدادات الأساسية
                    .Enrich.FromLogContext()                      // بيفتح الباب لاسم اليوزر من الميدل وير
                    .Enrich.WithMachineName()                     // بيسحب اسم جهاز السيرفر
                    .Enrich.WithThreadId());                      // بيسحب رقم الـ Thread الحالي




            var app = builder.Build();



            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<Application>();
                    var logger = services.GetRequiredService<ILogger<Program>>(); // سحبنا اللوجر هنا عشان يشتغل صح

                    // التشيك الذكي بتاعك
                    if (!context.Database.CanConnect())
                    {
                        logger.LogCritical("CRITICAL ERROR: Cannot connect to SQL Server database! Application startup aborted.");
                        throw new Exception("Database connection failed during startup.");
                    }
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogCritical(ex, "FATAL: Database crash detected on startup!");
                    throw;
                }
            }




            if (app.Environment.IsDevelopment())
             {
                app.UseSwagger();
                app.UseSwaggerUI();
            
             }

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await SeedData.CreateRoles(services);
                await SeedData.CreateAdmin(services);
            }
            app.UseExceptionHandler();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("AllowAll");
            app.UseRouting();

             app.UseAuthentication();
            app.UseAuthorization();

             app.UseMiddleware<UserLoggingMiddleware>();
            app.UseSerilogRequestLogging();
            app.UseRateLimiter();
            app.MapControllers();

            app.Run();
        }
    }
}


//< ItemGroup >
//    < PackageReference Include = "FluentValidation.AspNetCore" Version = "11.3.1" />
//    < PackageReference Include = "FluentValidation.DependencyInjectionExtensions" Version = "12.1.1" />
//    < PackageReference Include = "Microsoft.AspNetCore.Authentication.JwtBearer" Version = "8.0.0" />
//   < PackageReference Include = "Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version = "8.0.0" />
//   < PackageReference Include = "Microsoft.EntityFrameworkCore.Design" Version = "8.0.0" >
//     < PrivateAssets > all </ PrivateAssets >
//     < IncludeAssets > runtime; build; native; contentfiles; analyzers; buildtransitive </ IncludeAssets >
//   </ PackageReference >
//   < PackageReference Include = "Microsoft.EntityFrameworkCore.SqlServer" Version = "8.0.0" />
//   < PackageReference Include = "Microsoft.EntityFrameworkCore.Tools" Version = "8.0.0" >
//     < PrivateAssets > all </ PrivateAssets >
//     < IncludeAssets > runtime; build; native; contentfiles; analyzers; buildtransitive </ IncludeAssets >
//   </ PackageReference >

//     < PackageReference Include = "AutoMapper" Version = "16.0.0" />


//     < PackageReference Include = "AutoMapper.Extensions.Microsoft.DependencyInjection" Version = "12.0.1" />


//     < PackageReference Include = "Microsoft.Extensions.Caching.Hybrid" Version = "10.6.0" />
//   < PackageReference Include = "Mscc.GenerativeAI" Version = "3.1.0" />
//   < PackageReference Include = "Serilog.AspNetCore" Version = "10.0.0" />
//   < PackageReference Include = "Serilog.Enrichers.Environment" Version = "3.0.1" />
//   < PackageReference Include = "Serilog.Enrichers.OpenTelemetry" Version = "1.0.1" />
//   < PackageReference Include = "Serilog.Enrichers.Thread" Version = "4.0.0" />
//   < PackageReference Include = "Swashbuckle.AspNetCore" Version = "6.6.2" />
//   < PackageReference Include = "System.IdentityModel.Tokens.Jwt" Version = "7.0.3" />
// </ ItemGroup >
