using FluentValidation;
using System;

namespace BuildingManager.API.Application.Features.Residents.Commands.AssignResident;

public class AssignResidentCommandValidator : AbstractValidator<AssignResidentCommand>
{
    public AssignResidentCommandValidator()
    {
        RuleFor(v => v.UnitId)
            .GreaterThan(0);

        RuleFor(v => v.ResidentPhoneNumber) // اصلاح شد: از ResidentPhoneNumber استفاده می‌کند
            .NotEmpty().WithMessage("شماره موبایل ساکن الزامی است.")
            .Length(11).WithMessage("شماره موبایل باید 11 رقم باشد.");

        RuleFor(v => v.ResidentFullName)
            .NotEmpty();

        RuleFor(v => v.StartDate)
            .NotEmpty().WithMessage("تاریخ شروع سکونت الزامی است.");

        RuleFor(v => v.EndDate)
            .GreaterThan(v => v.StartDate)
            .When(v => v.EndDate.HasValue)
            .WithMessage("تاریخ پایان باید بعد از تاریخ شروع باشد.");
    }
}