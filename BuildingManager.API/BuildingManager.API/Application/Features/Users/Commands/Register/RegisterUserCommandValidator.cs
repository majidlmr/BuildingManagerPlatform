using FluentValidation;

namespace BuildingManager.API.Application.Features.Users.Commands.Register;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(v => v.FullName)
            .NotEmpty().WithMessage("نام و نام خانوادگی نمی‌تواند خالی باشد.")
            .MaximumLength(150);

        RuleFor(v => v.PhoneNumber) // اصلاح شد: از PhoneNumber استفاده می‌کند
            .NotEmpty().WithMessage("شماره موبایل نمی‌تواند خالی باشد.")
            .Length(11).WithMessage("شماره موبایل باید 11 رقم باشد.");

        RuleFor(v => v.Password)
            .NotEmpty().WithMessage("رمز عبور نمی‌تواند خالی باشد.")
            .MinimumLength(8).WithMessage("رمز عبور باید حداقل 8 کاراکتر باشد.");
    }
}