using FluentValidation;
using System;

namespace BuildingManager.API.Application.Features.Billing.Commands.CreateCycle;

public class CreateBillingCycleCommandValidator : AbstractValidator<CreateBillingCycleCommand>
{
    public CreateBillingCycleCommandValidator()
    {
        RuleFor(v => v.BuildingId)
            .GreaterThan(0);

        RuleFor(v => v.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(v => v.StartDate)
            .NotEmpty().WithMessage("تاریخ شروع دوره الزامی است.");

        RuleFor(v => v.EndDate)
            .NotEmpty().WithMessage("تاریخ پایان دوره الزامی است.")
            .GreaterThan(v => v.StartDate).WithMessage("تاریخ پایان باید بعد از تاریخ شروع باشد.");

        RuleFor(v => v.DefaultChargePerUnit)
            .GreaterThanOrEqualTo(0).WithMessage("مبلغ شارژ ثابت نمی‌تواند منفی باشد.");

        RuleFor(v => v.RequestingUserId)
            .GreaterThan(0);
    }
}