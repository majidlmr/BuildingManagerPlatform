using FluentValidation;
using BuildingManager.API.Domain.Entities; // For UnitType enum

namespace BuildingManager.API.Application.Features.Units.Commands.CreateUnit
{
    public class CreateUnitCommandValidator : AbstractValidator<CreateUnitCommand>
    {
        public CreateUnitCommandValidator()
        {
            RuleFor(x => x.BlockPublicId)
                .NotEmpty().WithMessage("شناسه عمومی بلوک اجباری است.");

            RuleFor(x => x.UnitNumber)
                .NotEmpty().WithMessage("شماره واحد اجباری است.")
                .MaximumLength(20).WithMessage("شماره واحد نمی‌تواند بیشتر از ۲۰ کاراکتر باشد.");

            RuleFor(x => x.Area)
                .GreaterThan(0).WithMessage("متراژ باید بیشتر از صفر باشد.");

            RuleFor(x => x.UnitType)
                .IsInEnum().WithMessage("نوع واحد نامعتبر است.");

            RuleFor(x => x.FloorNumber)
                .NotNull().When(x => x.UnitType == UnitType.Apartment || x.UnitType == UnitType.Office) // Example: floor number might be more relevant for apartments/offices
                .WithMessage("شماره طبقه برای این نوع واحد اجباری است.");
                // Add more specific floor number validations if needed (e.g. within block's floor range)

            RuleFor(x => x.NumberOfBedrooms)
                .GreaterThanOrEqualTo(0).When(x => x.NumberOfBedrooms.HasValue)
                .WithMessage("تعداد اتاق خواب نمی‌تواند منفی باشد.")
                .NotNull().When(x => x.UnitType == UnitType.Apartment) // Example: Bedrooms required for apartments
                .WithMessage("تعداد اتاق خواب برای آپارتمان اجباری است.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("توضیحات نمی‌تواند بیشتر از ۵۰۰ کاراکتر باشد.");
        }
    }
}
