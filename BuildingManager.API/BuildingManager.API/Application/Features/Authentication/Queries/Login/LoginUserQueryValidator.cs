using FluentValidation;

namespace BuildingManager.API.Application.Features.Authentication.Queries.Login
{
    public class LoginUserQueryValidator : AbstractValidator<LoginUserQuery>
    {
        public LoginUserQueryValidator()
        {
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("شماره موبایل اجباری است.")
                .Matches(@"^(09\d{9})$").WithMessage("فرمت شماره موبایل نامعتبر است.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("رمز عبور اجباری است.");
        }
    }
}
