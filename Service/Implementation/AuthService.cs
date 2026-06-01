
using Humanizer;
using System.Security.Authentication;

namespace E_Commerce_API.Service.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly Application _db;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly JWTService _jwtService;  
        private readonly ILogger<AuthService> logger;

        public AuthService(UserManager<User> userManager, IMapper mapper, JWTService jwtService, Application db, ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _mapper = mapper;
            _jwtService = jwtService;
            _db = db;
            this.logger = logger;
        }



        public async Task<List<UsersDTO>> GetAllUsers(CancellationToken ct = default)
        {
            var users = await _db.Users.Where(a => !a.IsDeleted).ToListAsync(ct);
            var map = _mapper.Map<List<UsersDTO>>(users);
            return map;
        }


        public async Task<string> DeleteUser(string email)
        {
            var user = await _db.Users.FirstOrDefaultAsync(a => a.Email == email);
            if (user == null)
                return "Email Not Found";
            if (user.IsDeleted)
                return "Email Already Deleted";
            user.IsDeleted = true;
             _db.SaveChanges();
            logger.LogInformation("Delete User {email} Successfuly At {Time}.",email, DateTime.Now);
            return "Deleted Successfuly";

        }

        // 1️⃣ تعديل دالة الـ Register عشان ترجع الـ AuthResponseDTO
        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO registerDto)
        {
            var findEmail = await _userManager.FindByEmailAsync(registerDto.Email);
            if (findEmail != null)
                throw new InvalidOperationException("your email already exists");

            var newUser = _mapper.Map<User>(registerDto);
            newUser.UserName = registerDto.Email;

            var result = await _userManager.CreateAsync(newUser, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new ArgumentException(errors);
            }

            await _userManager.AddToRoleAsync(newUser, "User");

            // 🔥 التعديل هنا: بنستقبل الـ Tuple (الـ Access والـ Refresh)
            var (accessToken, refreshToken) = await _jwtService.GenerateTokenAsync(newUser);
            logger.LogInformation("User {UserName} Registered  - At {Time}", registerDto.UserName,DateTime.Now);
             return new AuthResponseDTO
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                IsAuthenticated = true,
                Message = "User Registered Successfully",
                Username = newUser.UserName,
                Email = newUser.Email,
                Roles = new List<string> { "User" }
            };
        }

        // 2️⃣ تعديل دالة الـ Login عشان ترجع الـ AuthResponseDTO
        public async Task<AuthResponseDTO> LoginAsync(LoginDTO loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                logger.LogWarning("Login failed for user {Email}. Reason: Wrong Email. At {Time}", loginDto.Email,DateTime.Now);
               throw new AuthenticationException("Invalid Email or password");
            }

            var validPassword = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!validPassword)
            {
                logger.LogWarning("Login failed for user {Email}. Reason: Wrong password. At {Time}", loginDto.Email,DateTime.Now);
               throw new AuthenticationException("Invalid Email or password");
            }

            // 🔥 التعديل هنا: بنفك الـ Tuple اللي راجع من السيرفيس المساعدة
            var (accessToken, refreshToken) = await _jwtService.GenerateTokenAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            logger.LogInformation("Email {UserName} Logged in  - At {Time}", loginDto.Email, DateTime.Now);
            return new AuthResponseDTO
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                IsAuthenticated = true,
                Message = "Login Successful",
                Username = user.UserName,
                Email = user.Email,
                Roles = new List<string>(roles)
            };
        }

        public async Task PromoteToAdminAsync(string userEmail, string currentAdminEmail)
        {
            // 1. البحث عن اليوزر المراد ترقيته
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            // 2. تشيك الأمان: ممنوع ترقي نفسك
            if (user.Email == currentAdminEmail)
                throw new InvalidOperationException("You cannot promote yourself");

            // 3. شيله من دور الـ User لو موجود
            if (await _userManager.IsInRoleAsync(user, "User"))
            {
                var removeResult = await _userManager.RemoveFromRoleAsync(user, "User");
                if (!removeResult.Succeeded)
                    throw new ArgumentException("Failed to remove user from current role");
            }

            // 4. ضيفه للـ Admin لو مش موجود
            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                var addResult = await _userManager.AddToRoleAsync(user, "Admin");
                if (!addResult.Succeeded)
                    throw new ArgumentException("Failed to add user to Admin role");
            }
            logger.LogInformation("Admin{currentAdminEmail} promote User{UserEmail} To Admin At{Time}", currentAdminEmail,userEmail,DateTime.Now);
        }





    }
}
