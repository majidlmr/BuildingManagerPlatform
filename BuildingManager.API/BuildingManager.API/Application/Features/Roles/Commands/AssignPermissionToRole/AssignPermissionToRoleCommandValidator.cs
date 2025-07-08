using FluentValidation;

namespace BuildingManager.API.Application.Features.Roles.Commands.AssignPermissionToRole;

public class AssignPermissionToRoleCommandValidator : AbstractValidator<AssignPermissionToRoleCommand>
{
    public AssignPermissionToRoleCommandValidator()
    {
        RuleFor(v => v.RoleId).GreaterThan(0);
        RuleFor(v => v.BuildingId).GreaterThan(0);
        RuleFor(v => v.RequestingUserId).GreaterThan(0);
        RuleFor(v => v.PermissionIds)
            .NotEmpty().WithMessage("حداقل یک دسترسی باید انتخاب شود.");
    }
}