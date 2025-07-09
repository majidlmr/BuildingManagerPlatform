using FluentValidation;

namespace BuildingManager.API.Application.Features.Authentication.Commands.ConfirmPhoneNumber
{
    public class ConfirmPhoneNumberCommandValidator : AbstractValidator<ConfirmPhoneNumberCommand>
    {
        public ConfirmPhoneNumberCommandValidator()
        {
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("شماره موبایل اجباری است.")
                .Matches(@"^(09\d{9})$").WithMessage("فرمت شماره موبایل نامعتبر است (مثال: 09123456789).");

            RuleFor(x => x.OtpCode)
                .NotEmpty().WithMessage("کد تایید اجباری است.")
                .Length(6).WithMessage("کد تایید باید ۶ رقم باشد.")
                .Matches("^[0-9]*$").WithMessage("کد تایید فقط می‌تواند شامل اعداد باشد.");
        }
    }
}
