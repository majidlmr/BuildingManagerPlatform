using MediatR;
using System.Collections.Generic;
namespace BuildingManager.API.Application.Features.Roles.Queries.GetBuildingRoles;
public record GetBuildingRolesQuery(int BuildingId, int RequestingUserId) : IRequest<List<RoleDto>>;