using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Rules.Queries.GetRules;

public class GetRulesQueryHandler : IRequestHandler<GetRulesQuery, List<RuleDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuthorizationService _authorizationService; // 👈 سرویس دسترسی

    public GetRulesQueryHandler(IApplicationDbContext context, IAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService; // 👈 مقداردهی
    }

    public async Task<List<RuleDto>> Handle(GetRulesQuery request, CancellationToken cancellationToken)
    {
        // ✅ TODO تکمیل شد: بررسی اینکه آیا کاربر عضو ساختمان است یا خیر
        var isMember = await _authorizationService.IsMemberOfBuildingAsync(request.RequestingUserId, request.BuildingId, cancellationToken);
        if (!isMember)
        {
            throw new ForbiddenAccessException("شما اجازه دسترسی به قوانین این ساختمان را ندارید.");
        }

        // ادامه منطق بدون تغییر
        var rules = await _context.BuildingRules
            .Where(r => r.BuildingId == request.BuildingId && r.IsActive)
            .Select(r => new RuleDto(
                r.Id,
                r.Title,
                r.Content,
                r.Acknowledgments.Any(a => a.UserId == request.RequestingUserId)
            ))
            .ToListAsync(cancellationToken);

        return rules;
    }
}