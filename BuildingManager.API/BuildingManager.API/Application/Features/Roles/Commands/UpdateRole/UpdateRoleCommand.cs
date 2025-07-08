using MediatR;
namespace BuildingManager.API.Application.Features.Roles.Commands.UpdateRole;

public record UpdateRoleCommand(int BuildingId, int RoleId, string NewName, int RequestingUserId) : IRequest;