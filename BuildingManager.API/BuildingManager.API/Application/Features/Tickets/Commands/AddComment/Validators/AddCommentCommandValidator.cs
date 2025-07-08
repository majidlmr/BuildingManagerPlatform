using FluentValidation;

namespace BuildingManager.API.Application.Features.Tickets.Commands.AddComment;

public class AddCommentCommandValidator : AbstractValidator<AddCommentCommand>
{
    public AddCommentCommandValidator()
    {
        RuleFor(v => v.TicketPublicId)
            .NotEmpty();

        RuleFor(v => v.Comment)
            .NotEmpty().WithMessage("متن کامنت نمی‌تواند خالی باشد.");

        RuleFor(v => v.UserId)
            .GreaterThan(0);
    }
}