 
namespace E_Commerce_API.DTO.Identity
{
    public class RegisterDTO
    {
        public string FullName { get; set; }
        public string Country { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }


         

    }
}
