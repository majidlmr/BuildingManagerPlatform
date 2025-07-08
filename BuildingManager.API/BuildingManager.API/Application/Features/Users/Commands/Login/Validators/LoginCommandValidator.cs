using FluentValidation;

namespace BuildingManager.API.Application.Features.Users.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(v => v.PhoneNumber) // اصلاح شد: از PhoneNumber استفاده می‌کند
            .NotEmpty().WithMessage("شماره موبایل نمی‌تواند خالی باشد.");

        RuleFor(v => v.Password)
            .NotEmpty().WithMessage("رمز عبور نمی‌تواند خالی باشد.");
    }
}