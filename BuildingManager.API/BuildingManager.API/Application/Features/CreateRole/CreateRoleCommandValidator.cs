using FluentValidation;

namespace BuildingManager.API.Application.Features.Roles.Commands.CreateRole;

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(v => v.BuildingId).GreaterThan(0);
        RuleFor(v => v.RoleName).NotEmpty().MaximumLength(100);
        RuleFor(v => v.RequestingUserId).GreaterThan(0);
    }
}