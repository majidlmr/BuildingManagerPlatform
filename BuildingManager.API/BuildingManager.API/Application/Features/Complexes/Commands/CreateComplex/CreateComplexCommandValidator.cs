using FluentValidation;
using BuildingManager.API.Domain.Entities; // For ComplexType enum

namespace BuildingManager.API.Application.Features.Complexes.Commands.CreateComplex
{
    public class CreateComplexCommandValidator : AbstractValidator<CreateComplexCommand>
    {
        public CreateComplexCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("نام مجتمع اجباری است.")
                .MaximumLength(200).WithMessage("نام مجتمع نمی‌تواند بیشتر از ۲۰۰ کاراکتر باشد.");

            RuleFor(x => x.Address)
                .MaximumLength(500).WithMessage("آدرس نمی‌تواند بیشتر از ۵۰۰ کاراکتر باشد.");

            RuleFor(x => x.ComplexType)
                .IsInEnum().WithMessage("نوع مجتمع نامعتبر است.");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90).When(x => x.Latitude.HasValue).WithMessage("عرض جغرافیایی نامعتبر است.");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180).When(x => x.Longitude.HasValue).WithMessage("طول جغرافیایی نامعتبر است.");

            RuleFor(x => x.RulesFileUrl)
                .MaximumLength(500).WithMessage("آدرس فایل قوانین نمی‌تواند بیشتر از ۵۰۰ کاراکتر باشد.")
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .When(x => !string.IsNullOrEmpty(x.RulesFileUrl))
                .WithMessage("آدرس فایل قوانین باید یک URL معتبر باشد.");

            // Amenities can be a JSON string, so specific validation might be complex here.
            // Basic length check or JSON format check could be added if necessary.
        }
    }
}
