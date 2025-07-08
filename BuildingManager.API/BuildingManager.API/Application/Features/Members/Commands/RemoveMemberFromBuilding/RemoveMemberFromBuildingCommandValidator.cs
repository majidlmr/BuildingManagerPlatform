using FluentValidation;

namespace BuildingManager.API.Application.Features.Members.Commands.RemoveMemberFromBuilding;

public class RemoveMemberFromBuildingCommandValidator : AbstractValidator<RemoveMemberFromBuildingCommand>
{
    public RemoveMemberFromBuildingCommandValidator()
    {
        RuleFor(v => v.BuildingId).GreaterThan(0);
        RuleFor(v => v.MemberUserId).GreaterThan(0);
        RuleFor(v => v.RequestingUserId).GreaterThan(0);
    }
}