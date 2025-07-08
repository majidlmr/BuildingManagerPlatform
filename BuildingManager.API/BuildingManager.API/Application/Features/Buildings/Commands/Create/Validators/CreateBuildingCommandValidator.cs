using FluentValidation;

namespace BuildingManager.API.Application.Features.Buildings.Commands.Create;

public class CreateBuildingCommandValidator : AbstractValidator<CreateBuildingCommand>
{
    public CreateBuildingCommandValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("نام ساختمان نمی‌تواند خالی باشد.")
            .MaximumLength(200).WithMessage("نام ساختمان نمی‌تواند بیشتر از 200 کاراکتر باشد.");

        RuleFor(v => v.BuildingType)
            .NotEmpty().WithMessage("نوع ساختمان نمی‌تواند خالی باشد.")
            .MaximumLength(50);

        RuleFor(v => v.OwnerUserId)
            .GreaterThan(0).WithMessage("شناسه مالک ساختمان باید معتبر باشد.");
    }
}