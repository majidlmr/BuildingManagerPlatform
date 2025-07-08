using FluentValidation;

namespace BuildingManager.API.Application.Features.Units.Commands.AddUnit;

public class AddUnitCommandValidator : AbstractValidator<AddUnitCommand>
{
    public AddUnitCommandValidator()
    {
        RuleFor(v => v.UnitNumber)
            .NotEmpty().WithMessage("شماره واحد نمی‌تواند خالی باشد.")
            .MaximumLength(20);

        RuleFor(v => v.OwnershipStatus)
            .NotEmpty().WithMessage("وضعیت مالکیت نمی‌تواند خالی باشد.")
            .MaximumLength(50);

        RuleFor(v => v.BuildingId)
            .GreaterThan(0).WithMessage("شناسه ساختمان باید معتبر باشد.");

        RuleFor(v => v.OwnerUserId)
            .GreaterThan(0).WithMessage("شناسه مالک واحد باید معتبر باشد.");

        RuleFor(v => v.RequestingUserId)
            .GreaterThan(0);
    }
}