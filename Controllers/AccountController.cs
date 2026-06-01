

using Microsoft.AspNetCore.RateLimiting;
using System.Security.Authentication;

namespace E_Commerce_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("CountRequest")]  
    public class AccountController : ControllerBase
    {

        private readonly  IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly JWTService _jwtService;
         private readonly IAuthService _authService;
        private readonly Application _context;

        public AccountController(AuthService users, JWTService jwtService,
                                 IMapper mapper, UserManager<User> userManager,
                                  SignInManager<User> signInManager, IAuthService authService, Application context)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _authService = authService;
            _context = context;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // 🎯 التعديل هنا: بنستقبل الـ AuthResponseDTO كامل مش مجرد string
                var result = await _authService.RegisterAsync(registerDto);

                // بنرجع الكلاس كامل بـ كل الداتا اللي جواه (التوكينات، اسم اليوزر، الرولز)
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message }); // 409 Conflict
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message }); // 400 BadRequest
            }
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // 🎯 التعديل هنا: بنستقبل الـ AuthResponseDTO كامل
                var result = await _authService.LoginAsync(loginDto);

                // بنرجع الـ object كامل للـ Frontend
                return Ok(result);
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(new { message = ex.Message }); // 401 Unauthorized
            }
        }

        [HttpPost("AddRole")]
        public async Task<IActionResult> PromoteToAdmin(string userEmail)
        {
            // بنجيب إيميل الأدمن الحالي اللي عامل Login من الـ Token
            var currentAdminEmail = User.Identity?.Name;

            try
            {
                // نداء السيرفيس
                await _authService.PromoteToAdminAsync(userEmail, currentAdminEmail);
                return Ok(new { message = $"User {userEmail} is now an Admin" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); // 404
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message }); // 400
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message }); // 400
            }
        }


        [EndpointSummary("Refresh Token")]
        [EndpointDescription("Refresh Token Return new Access Token And Refresh Token")]

        [HttpPost("Refresh")]
        public async Task<IActionResult> RefreshToken(string refreshtoken)
        {
            var tokenRecord = await _context.RefreshToken.FirstOrDefaultAsync(a => a.Token == refreshtoken);

            if (tokenRecord == null)
                return Unauthorized("this token invalid");

            if (tokenRecord.Expires < DateTime.UtcNow)
                return Unauthorized("this token is expired");

            if (tokenRecord.IsRevoked)
                return Unauthorized("this token is cancelled");

            var user = await _userManager.FindByIdAsync(tokenRecord.UserId);
            if (user == null)
                return NotFound("this user invalid");

            // 1. بنمسح التوكن القديم اللي استخدمناه حالا (Token Rotation عشان الأمان والداتا بيز)
            _context.RefreshToken.Remove(tokenRecord);
            await _context.SaveChangesAsync();

            // 2. بنولد التوكينات الجديدة (الاكسس والريفرش الجداد)
            var (accessToken, newRefreshToken) = await _jwtService.GenerateTokenAsync(user);

            // 3. بنجيب الرولز عشان نملى الكلاس الكبير اللطيف بتاعنا
            var roles = await _userManager.GetRolesAsync(user);

            // 4. بنرجع الـ AuthResponseDTO كامل للـ Frontend
            return Ok(new AuthResponseDTO
            {
                Token = accessToken,
                RefreshToken = newRefreshToken,
                IsAuthenticated = true,
                Message = "Token Refreshed Successfully",
                Username = user.UserName,
                Email = user.Email,
                Roles = roles.ToList()
            });
        }

        [EndpointSummary("LogOut From System")]
        [EndpointDescription("LogOut From System")]

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(string refreshToken)
        {
            var tokenRecord = await _context.RefreshToken.FirstOrDefaultAsync(a => a.Token == refreshToken);
            if (tokenRecord == null)
            {
                return BadRequest(new { message = "Invalid Refresh Token. Logout failed." });
            }
            
                // 🎯 التعديل هنا: بنستخدم Remove لأنها بروبرتي واحدة مش لستة
                _context.RefreshToken.Remove(tokenRecord);
                await _context.SaveChangesAsync(); // استخدم Async أفضل
            return Ok(new { message = "Logout Successfully" });
        }


        [HttpGet("AllUsers")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers(CancellationToken ct = default)
        {
            var users = await _authService.GetAllUsers(ct);
             return Ok(users);
        }



        [HttpPut("DeleteUser")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string email)
        {
            var result = await _authService.DeleteUser(email);

            return result switch
            {
                "Email Not Found" => NotFound("Email Not Found"),
                "Email Already Deleted" => BadRequest("Email For User is Already Deleted"),
                "Deleted Successfuly" => Ok("User Deleted Successfuly"),
                _ => StatusCode(500, "An unexpected error occurred") // حماية لو حصلت حاجة مش متوقعة
            };
        }






         



    }
}
