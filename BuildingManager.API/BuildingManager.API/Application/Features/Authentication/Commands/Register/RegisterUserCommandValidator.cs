using FluentValidation;

namespace BuildingManager.API.Application.Features.Authentication.Commands.Register
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("نام اجباری است.")
                .MaximumLength(75).WithMessage("نام نمی‌تواند بیشتر از ۷۵ کاراکتر باشد.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("نام خانوادگی اجباری است.")
                .MaximumLength(75).WithMessage("نام خانوادگی نمی‌تواند بیشتر از ۷۵ کاراکتر باشد.");

            RuleFor(x => x.NationalId)
                .NotEmpty().WithMessage("کد ملی اجباری است.")
                .Length(10).WithMessage("کد ملی باید ۱۰ رقم باشد.")
                .Matches("^[0-9]*$").WithMessage("کد ملی فقط می‌تواند شامل اعداد باشد.");
                // Add custom validator for National ID checksum if needed

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("شماره موبایل اجباری است.")
                .MaximumLength(15).WithMessage("شماره موبایل نامعتبر است.")
                .Matches(@"^(09\d{9})$").WithMessage("فرمت شماره موبایل نامعتبر است (مثال: 09123456789).");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("رمز عبور اجباری است.")
                .MinimumLength(8).WithMessage("رمز عبور باید حداقل ۸ کاراکتر باشد.")
                .Matches("[A-Z]").WithMessage("رمز عبور باید شامل حداقل یک حرف بزرگ باشد.")
                .Matches("[a-z]").WithMessage("رمز عبور باید شامل حداقل یک حرف کوچک باشد.")
                .Matches("[0-9]").WithMessage("رمز عبور باید شامل حداقل یک عدد باشد.")
                .Matches("[^a-zA-Z0-9]").WithMessage("رمز عبور باید شامل حداقل یک کاراکتر خاص باشد.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("تکرار رمز عبور اجباری است.")
                .Equal(x => x.Password).WithMessage("رمز عبور و تکرار آن یکسان نیستند.");
        }
    }
}
