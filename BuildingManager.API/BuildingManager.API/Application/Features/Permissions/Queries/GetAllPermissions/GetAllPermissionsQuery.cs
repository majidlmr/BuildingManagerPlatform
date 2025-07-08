using MediatR;
using System.Collections.Generic;

namespace BuildingManager.API.Application.Features.Permissions.Queries.GetAllPermissions;

public class GetAllPermissionsQuery : IRequest<List<PermissionDto>> { }