namespace E_Commerce_API.Service.Interface
{
    public interface IAuthService
    {
         Task<List<UsersDTO>> GetAllUsers(CancellationToken ct = default);
        Task<string> DeleteUser(string email);
         


        Task<AuthResponseDTO> RegisterAsync(RegisterDTO registerDto);
         Task<AuthResponseDTO> LoginAsync(LoginDTO loginDto);

        Task PromoteToAdminAsync(string userEmail, string currentAdminEmail);
    }
}
