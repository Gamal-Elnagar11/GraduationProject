using E_Commerce_API.ErrorHandling;
using System.Reflection;
using System.Threading.RateLimiting;

namespace E_Commerce_API.DependencyInjection
{
    public static class DependencyInjectionService
    {
        public static IServiceCollection DIService(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddScoped<JWTService>();
            service.AddScoped<IUnitOfWork, UnitOfWork_Implement>();
            service.AddScoped<IProductService, ProductService>();
            service.AddScoped<ICategoryService, CategoryService>();
            service.AddScoped<ICartService, CartService>();
            service.AddScoped<IOrderService, OrderService>();
            service.AddScoped<IOrderRepo, OrderRepo>();
            service.AddScoped<IFAQService, FAQService>();
            service.AddScoped<IFeedbackService, FeedbackService>();
            service.AddScoped<AuthService>();
            service.AddAuthorization();
            service.AddHttpClient();
            service.AddHttpContextAccessor();
            service.AddScoped<IAuthService, AuthService>();

            service.AddProblemDetails(); // مهمة جداً عشان تشغل الـ IProblemDetailsService
            service.AddExceptionHandler<GlobalErrorHandling>();

           




            service.AddRateLimiter(options =>
            {
                // الكود اللي هيرجع لليوزر لما يتخطى الحد المسموح
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddPolicy("CountRequest", context =>
                {
                    // تحديد هوية المستخدم بناءً على الـ ID أو الـ IP
                    var key = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                              ?? context.Connection.RemoteIpAddress?.ToString()
                              ?? "anonymous";

                    return RateLimitPartition.GetSlidingWindowLimiter(key, _ => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromMinutes(1),
                        SegmentsPerWindow = 4,
                        QueueLimit = 2,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst // 👈 تنظيم الويتنج بالترتيب
                    });
                });
            });



            var jwtSettings = configuration.GetSection("JWT");

            //  Authentication
            service.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,                     // تحقق من الـ Issuer
                    ValidateAudience = true,                   // تحقق من الـ Audience
                    ValidateLifetime = true,                    // تحقق من انتهاء الصلاحية
                    ValidateIssuerSigningKey = true,           // تحقق من التوقيع
                    ValidIssuer = jwtSettings["Issuer"],       // من appsettings.json
                    ValidAudience = jwtSettings["Audience"],   // من appsettings.json
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]))
                };
            });


            // AutoMapper
            service.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());
            service.AddIdentity<User, IdentityRole>()
               .AddEntityFrameworkStores<Application>()
               .AddDefaultTokenProviders();



            // Serialization
            service.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters
                    .Add(new JsonStringEnumConverter());
            });
             service.AddEndpointsApiExplorer();
            

            service.ConfigureApplicationCookie(options =>
            {
                // لو حاول يروح Login redirect خلي الاستجابة 401
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };

                // لو حاول يروح AccessDenied redirect خلي الاستجابة 403
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                };
            });


            service.AddAuthorization(options =>
            {
                options.AddPolicy("UserOrAdmin", policy =>
                    policy.RequireRole("User", "Admin"));
            });


            // Swagger info
            service.AddSwaggerGen(opt =>
            {


                opt.OperationFilter<OpenApiSpecification>();
                opt.CustomOperationIds(apiDesc =>
                {
                    var name = apiDesc.ActionDescriptor.AttributeRouteInfo?.Name;
                    if (!string.IsNullOrEmpty(name)) return name;
                    return apiDesc.RelativePath?.Split('/').Last();
                });


                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });
                // here create jwt in header for evvery endpoint
                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });

                opt.SwaggerDoc("v1", new OpenApiInfo()   // here V1 -> version 1 to api after audated on this you can set V2
                {
                    Version = "v1",
                    Title = "Graduation Project",
                    Description = "Graduation Project Ecommerce & FAQ",
                    TermsOfService = new Uri("http://tempuri.gamal"),    // here linke to page has info to api
                    Contact = new OpenApiContact                         // here info to owner
                    {
                        Name = "Gamal saeed",
                        Email = "gamalelnagar@gmail.com"
                    }

                });
            });
             


            // Cors
            service.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });


            return service;

        }
    }
}
