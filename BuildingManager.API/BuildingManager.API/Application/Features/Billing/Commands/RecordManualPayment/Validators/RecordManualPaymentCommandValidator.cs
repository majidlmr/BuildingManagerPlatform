using FluentValidation;

namespace BuildingManager.API.Application.Features.Billing.Commands.RecordManualPayment;

public class RecordManualPaymentCommandValidator : AbstractValidator<RecordManualPaymentCommand>
{
    public RecordManualPaymentCommandValidator()
    {
        RuleFor(v => v.InvoicePublicId)
            .NotEmpty();

        RuleFor(v => v.Amount)
            .GreaterThan(0).WithMessage("مبلغ پرداخت باید بیشتر از صفر باشد.");

        RuleFor(v => v.PaidAt)
            .NotEmpty().WithMessage("تاریخ پرداخت الزامی است.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("تاریخ پرداخت نمی‌تواند در آینده باشد.");
    }
}