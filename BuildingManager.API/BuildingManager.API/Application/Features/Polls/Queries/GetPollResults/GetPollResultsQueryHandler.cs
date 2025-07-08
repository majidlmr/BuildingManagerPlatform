using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Polls.Queries.GetPollResults;

public class GetPollResultsQueryHandler : IRequestHandler<GetPollResultsQuery, PollResultDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuthorizationService _authorizationService; // 👈 سرویس دسترسی

    public GetPollResultsQueryHandler(IApplicationDbContext context, IAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService; // 👈 مقداردهی
    }

    public async Task<PollResultDto> Handle(GetPollResultsQuery request, CancellationToken cancellationToken)
    {
        var poll = await _context.Polls
            .AsNoTracking()
            .Include(p => p.Options)
            .ThenInclude(o => o.Votes)
            .FirstOrDefaultAsync(p => p.Id == request.PollId, cancellationToken);

        if (poll == null)
        {
            throw new NotFoundException("نظرسنجی یافت نشد.");
        }

        // ✅ TODO تکمیل شد: بررسی دسترسی کاربر به نتایج
        var isMember = await _authorizationService.IsMemberOfBuildingAsync(request.RequestingUserId, poll.BuildingId, cancellationToken);
        if (!isMember)
        {
            throw new ForbiddenAccessException("شما اجازه مشاهده نتایج این نظرسنجی را ندارید.");
        }

        // ادامه منطق بدون تغییر
        var totalVotes = poll.Options.Sum(o => o.Votes.Count);

        var optionsResults = poll.Options.Select(o => new PollOptionResultDto(
            o.Text,
            o.Votes.Count,
            totalVotes > 0 ? Math.Round((double)o.Votes.Count * 100 / totalVotes, 2) : 0
        )).ToList();

        return new PollResultDto
        {
            Question = poll.Question,
            TotalVotes = totalVotes,
            Options = optionsResults
        };
    }
}