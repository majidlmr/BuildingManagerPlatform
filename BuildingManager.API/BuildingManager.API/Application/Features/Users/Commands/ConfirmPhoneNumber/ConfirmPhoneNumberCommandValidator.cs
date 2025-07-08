using FluentValidation;

namespace BuildingManager.API.Application.Features.Users.Commands.ConfirmPhoneNumber;

/// <summary>
/// کلاس اعتبارسنجی برای دستور تایید شماره موبایل.
/// </summary>
public class ConfirmPhoneNumberCommandValidator : AbstractValidator<ConfirmPhoneNumberCommand>
{
    public ConfirmPhoneNumberCommandValidator()
    {
        RuleFor(v => v.PhoneNumber)
            .NotEmpty().WithMessage("شماره موبایل نمی‌تواند خالی باشد.")
            .Length(11).WithMessage("شماره موبایل باید 11 رقم باشد.");

        RuleFor(v => v.OtpCode)
            .NotEmpty().WithMessage("کد تایید الزامی است.")
            .Length(6).WithMessage("کد تایید باید 6 رقم باشد.");

        RuleFor(v => v.NewPassword)
            .NotEmpty().WithMessage("رمز عبور جدید نمی‌تواند خالی باشد.")
            .MinimumLength(8).WithMessage("رمز عبور باید حداقل 8 کاراکتر باشد.");
    }
}