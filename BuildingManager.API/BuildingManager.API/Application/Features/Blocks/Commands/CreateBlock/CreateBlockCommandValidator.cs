using FluentValidation;
using BuildingManager.API.Domain.Entities; // For BlockType enum
using System;

namespace BuildingManager.API.Application.Features.Blocks.Commands.CreateBlock
{
    public class CreateBlockCommandValidator : AbstractValidator<CreateBlockCommand>
    {
        public CreateBlockCommandValidator()
        {
            RuleFor(x => x.NameOrNumber)
                .NotEmpty().WithMessage("نام یا شماره بلوک/ساختمان اجباری است.")
                .MaximumLength(200).WithMessage("نام یا شماره بلوک/ساختمان نمی‌تواند بیشتر از ۲۰۰ کاراکتر باشد.");

            RuleFor(x => x.BlockType)
                .IsInEnum().WithMessage("نوع بلوک/ساختمان نامعتبر است.");

            RuleFor(x => x.Address)
                .MaximumLength(500).WithMessage("آدرس نمی‌تواند بیشتر از ۵۰۰ کاراکتر باشد.");

            RuleFor(x => x.ChargeCalculationStrategyName)
                .NotEmpty().WithMessage("استراتژی محاسبه شارژ اجباری است.")
                .MaximumLength(100).WithMessage("نام استراتژی شارژ طولانی است.");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90).When(x => x.Latitude.HasValue).WithMessage("عرض جغرافیایی نامعتبر است.");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180).When(x => x.Longitude.HasValue).WithMessage("طول جغرافیایی نامعتبر است.");

            RuleFor(x => x.RulesFileUrl)
                .MaximumLength(500).WithMessage("آدرس فایل قوانین نمی‌تواند بیشتر از ۵۰۰ کاراکتر باشد.")
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .When(x => !string.IsNullOrEmpty(x.RulesFileUrl))
                .WithMessage("آدرس فایل قوانین باید یک URL معتبر باشد.");

            RuleFor(x => x.NumberOfFloors)
                .GreaterThanOrEqualTo(0).When(x => x.NumberOfFloors.HasValue).WithMessage("تعداد طبقات نمی‌تواند منفی باشد.");

            RuleFor(x => x.TotalUnits)
                .GreaterThanOrEqualTo(0).When(x => x.TotalUnits.HasValue).WithMessage("تعداد واحدها نمی‌تواند منفی باشد.");

            RuleFor(x => x.ConstructionYear)
                .InclusiveBetween(1000, DateTime.UtcNow.Year + 5).When(x => x.ConstructionYear.HasValue) // +5 for buildings under construction
                .WithMessage("سال ساخت نامعتبر است.");
        }
    }
}
