using System.Collections.Generic;
namespace BuildingManager.API.Application.Features.Roles.Queries.GetBuildingRoles;
public record RoleDto(int Id, string Name, List<string> Permissions);