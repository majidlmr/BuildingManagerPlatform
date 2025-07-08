using FluentValidation;
namespace BuildingManager.API.Application.Features.Roles.Commands.UpdateRole;

public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator()
    {
        RuleFor(v => v.RoleId).GreaterThan(0);
        RuleFor(v => v.NewName).NotEmpty().MaximumLength(100);
    }
}