using MediatR;
namespace BuildingManager.API.Application.Features.Roles.Commands.DeleteRole;

public record DeleteRoleCommand(int BuildingId, int RoleId, int RequestingUserId) : IRequest;