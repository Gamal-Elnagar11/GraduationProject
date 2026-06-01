
namespace E_Commerce_API.Static
{
    public class JWTService
    {

        private readonly IConfiguration _configuration;
         private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<User> _userManager;
        private readonly Application _context;

        public JWTService(IConfiguration configuration, UserManager<User> userManager, Application context, RoleManager<IdentityRole> roleManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _context = context;
            this.roleManager = roleManager;
        }

        public async Task<(string AccessToken, string RefreshToken)> GenerateTokenAsync(User user)
        {

            // 1️⃣ نجيب الرولز الخاصة بالمستخدم
            var roles = await _userManager.GetRolesAsync(user);

            // 2️⃣ نجهز الـ Claims
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            // إضافة الرولز كـ Claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));

                var rolee = await roleManager.FindByNameAsync(role);
                if (rolee != null)
                {
                    var roleClaims = await roleManager.GetClaimsAsync(rolee);
                    claims.AddRange(roleClaims); // إضافة كل صلاحيات الرول للتوكن
                }
            }

            // 3️⃣ نجيب الـ Secret Key
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 4️⃣ إنشاء التوكن
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: creds
            );

            // here has token
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);




            // Refresh Token
            // here delete any token from DB
          //  var oldTokens = _context.RefreshToken.Where(a => a.UserId == user.Id);
          //  _context.RefreshToken.RemoveRange(oldTokens);

            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                Expires = DateTime.UtcNow.AddDays(3),
                UserId = user.Id
            };


            _context.RefreshToken.Add(refreshToken);
            await _context.SaveChangesAsync();



            // 5️⃣ تحويله لـ string
            return (accessToken, refreshToken.Token);
        }


        public async Task<string> GenerateTokenAsync2(User user)
        {
            // 1️⃣ نجيب الرولز الخاصة بالمستخدم
            var roles = await _userManager.GetRolesAsync(user);

            // 2️⃣ نجهز الـ Claims
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            // إضافة الرولز كـ Claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // 3️⃣ نجيب الـ Secret Key
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 4️⃣ إنشاء التوكن
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: creds
            );

            // 5️⃣ تحويله لـ string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

