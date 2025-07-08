using MediatR;
using System.Collections.Generic;

namespace BuildingManager.API.Application.Features.Rules.Queries.GetRules;

public record GetRulesQuery(int BuildingId, int RequestingUserId) : IRequest<List<RuleDto>>;