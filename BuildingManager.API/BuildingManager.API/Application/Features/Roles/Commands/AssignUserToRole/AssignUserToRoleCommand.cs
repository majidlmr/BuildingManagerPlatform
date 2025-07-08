using MediatR;
namespace BuildingManager.API.Application.Features.Roles.Commands.AssignUserToRole;
public record AssignUserToRoleCommand(int BuildingId, int RoleId, int UserIdToAssign, int RequestingUserId) : IRequest;