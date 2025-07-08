using FluentValidation;

namespace BuildingManager.API.Application.Features.Members.Commands.RemoveUserFromRole;

public class RemoveUserFromRoleCommandValidator : AbstractValidator<RemoveUserFromRoleCommand>
{
    public RemoveUserFromRoleCommandValidator()
    {
        RuleFor(v => v.BuildingId).GreaterThan(0);
        RuleFor(v => v.UserIdToRemove).GreaterThan(0);
        RuleFor(v => v.RoleId).GreaterThan(0);
        RuleFor(v => v.RequestingUserId).GreaterThan(0);
    }
}