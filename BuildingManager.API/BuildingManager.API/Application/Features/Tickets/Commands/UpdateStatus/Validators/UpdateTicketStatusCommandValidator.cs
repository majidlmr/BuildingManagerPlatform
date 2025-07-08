using FluentValidation;

namespace BuildingManager.API.Application.Features.Tickets.Commands.UpdateStatus;

public class UpdateTicketStatusCommandValidator : AbstractValidator<UpdateTicketStatusCommand>
{
    public UpdateTicketStatusCommandValidator()
    {
        RuleFor(v => v.PublicId)
            .NotEmpty().WithMessage("شناسه تیکت نمی‌تواند خالی باشد.");

        RuleFor(v => v.NewStatus)
            .NotEmpty().WithMessage("وضعیت جدید نمی‌تواند خالی باشد.")
            .MaximumLength(50);
    }
}