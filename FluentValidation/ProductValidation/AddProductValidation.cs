using FluentValidation;

namespace E_Commerce_API.FluentValidation.ProductValidation
{
    public class AddProductValidation : AbstractValidator<AddProductDTO>
    {
        public AddProductValidation()
        {
            RuleFor(a => a.Name)
                          .NotEmpty().WithMessage("Product Name is Required")
                          .Length(1, 100).WithMessage("Product Name must bettwen 1 and 100 carachter");

            RuleFor(a => a.Description)
                .NotEmpty().WithMessage("Descriptiopn is required");

            RuleFor(a => a.CategoryId)
                .NotEmpty().WithMessage("CategoryId is required");

            RuleFor(a => a.Stock)
                .NotNull().WithMessage("Stock is required")
                .GreaterThan(0).WithMessage("Stock must be greater than 0");

            RuleFor(a => a.Price)
                .NotNull().WithMessage("Price is required")
                .GreaterThan(0).WithMessage("Price must be greater than 0");


            RuleFor(x => x.Image)
                .Must(file => file == null || file.ContentType.StartsWith("image/"))
                .WithMessage("Invalid file type")

                .Must(file => file == null || file.Length <= 2_000_000)
                .WithMessage("File too large (Max 2MB)")

                .Must(file => file == null || new[] { ".png", ".jpg", ".jpeg" }.Contains(Path.GetExtension(file.FileName).ToLower()))
                .WithMessage("Only .png and .jpg images are allowed");

        }
    }
}
