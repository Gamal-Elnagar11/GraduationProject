using FluentValidation;

namespace E_Commerce_API.FluentValidation.IdentityValidation
{
    public class RegisterValidation :AbstractValidator<RegisterDTO>
    {
        public RegisterValidation()
        {
            RuleFor(a => a.FullName)
                   .NotEmpty().WithMessage(" Name is Required");
            
            RuleFor(a => a.Country)
                   .NotEmpty().WithMessage(" Country is Required");
            
            RuleFor(a => a.PhoneNumber)
                   .NotEmpty().WithMessage(" PhoneNumber is Required");

            RuleFor(a => a.DateOfBirth)
                   .NotEmpty().WithMessage(" DateOfBirth is Required");

            RuleFor(a => a.UserName)
                   .NotEmpty().WithMessage(" UserName is Required");
            RuleFor(a => a.Email)
                .NotEmpty().WithMessage("Email is required") // بديل [Required]
                .EmailAddress().WithMessage("Invalid email format"); // بديل [EmailAddress]

            RuleFor(a => a.Password)
                 .NotEmpty().WithMessage("Password is required") // بديل Required
                 .MinimumLength(6).WithMessage("Password must be at least 6 characters"); // تأمين من عندك
           
            RuleFor(a => a.ConfirmPassword)
                 .NotEmpty().WithMessage("Confirm Password is required") // بديل Required
                 .Equal(a => a.Password).WithMessage("The password and confirmation password do not match!"); // بديل Compare


        }
    }
}
