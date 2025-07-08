using FluentValidation;

namespace BuildingManager.API.Application.Features.Tickets.Commands.CreateTicket;

public class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
{
    public CreateTicketCommandValidator()
    {
        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("عنوان تیکت نمی‌تواند خالی باشد.")
            .MaximumLength(200).WithMessage("عنوان تیکت نمی‌تواند بیشتر از 200 کاراکتر باشد.");

        RuleFor(v => v.Description)
            .NotEmpty().WithMessage("توضیحات تیکت نمی‌تواند خالی باشد.");

        RuleFor(v => v.Category)
            .NotEmpty().WithMessage("دسته بندی تیکت نمی‌تواند خالی باشد.")
            .MaximumLength(100);

        RuleFor(v => v.Priority)
            .NotEmpty().WithMessage("اولویت تیکت نمی‌تواند خالی باشد.")
            .MaximumLength(50);

        RuleFor(v => v.BuildingId)
            .GreaterThan(0).WithMessage("شناسه ساختمان باید معتبر باشد.");

        RuleFor(v => v.ReportedByUserId)
            .GreaterThan(0).WithMessage("شناسه گزارش دهنده باید معتبر باشد.");
    }
}