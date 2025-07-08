using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Members.Queries.GetBuildingMembers;

public class GetBuildingMembersQueryHandler : IRequestHandler<GetBuildingMembersQuery, List<MemberDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuthorizationService _authorizationService;

    public GetBuildingMembersQueryHandler(IApplicationDbContext context, IAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService;
    }

    public async Task<List<MemberDto>> Handle(GetBuildingMembersQuery request, CancellationToken cancellationToken)
    {
        var canReadMembers = await _authorizationService.HasPermissionAsync(request.RequestingUserId, request.BuildingId, "Member.Read", cancellationToken);
        if (!canReadMembers)
        {
            throw new ForbiddenAccessException("شما اجازه مشاهده اعضای این ساختمان را ندارید.");
        }

        var members = await _context.Users
            .Where(u => u.UserRoles.Any(ur => ur.Role.BuildingId == request.BuildingId))
            .Select(u => new MemberDto(
                u.Id,
                u.FullName,
                u.PhoneNumber,
                u.UserRoles.Where(ur => ur.Role.BuildingId == request.BuildingId)
                           .Select(ur => ur.Role.Name)
                           .ToList()
            ))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return members;
    }
}