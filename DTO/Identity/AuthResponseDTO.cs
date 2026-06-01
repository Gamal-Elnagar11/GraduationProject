namespace E_Commerce_API.DTO.Identity
{
    public class AuthResponseDTO
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Message { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; } // 👈 لستة الرولز
    }
}
