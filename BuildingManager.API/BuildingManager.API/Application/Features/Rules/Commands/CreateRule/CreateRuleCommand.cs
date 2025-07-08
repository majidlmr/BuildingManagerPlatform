using MediatR;

namespace BuildingManager.API.Application.Features.Rules.Commands.CreateRule;

public record CreateRuleCommand(
    int BuildingId,
    string Title,
    string Content,
    int CreatedByUserId
) : IRequest<int>;