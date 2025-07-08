// File: Application/Features/Polls/Commands/SubmitVote/SubmitVoteCommandHandler.cs

using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Interfaces;
using FluentValidation; // ✅ این خط برای رفع خطا اضافه شد
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Polls.Commands.SubmitVote;

public class SubmitVoteCommandHandler : IRequestHandler<SubmitVoteCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitVoteCommandHandler(IApplicationDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SubmitVoteCommand request, CancellationToken cancellationToken)
    {
        var poll = await _context.Polls
            .Include(p => p.Options)
            .FirstOrDefaultAsync(p => p.Id == request.PollId, cancellationToken);

        if (poll == null || !poll.IsActive || (poll.EndDate.HasValue && poll.EndDate < DateTime.UtcNow))
        {
            throw new NotFoundException("نظرسنجی فعال نیست یا یافت نشد.");
        }

        if (!poll.IsMultipleChoice)
        {
            var hasVoted = await _context.Votes
                .AnyAsync(v => v.PollOption.PollId == request.PollId && v.UserId == request.UserId, cancellationToken);

            if (hasVoted)
            {
                // اکنون این خط بدون خطا کار می‌کند
                throw new ValidationException("شما قبلاً در این نظرسنجی شرکت کرده‌اید.");
            }
            if (request.OptionIds.Count > 1)
            {
                throw new ValidationException("در این نظرسنجی فقط می‌توانید یک گزینه را انتخاب کنید.");
            }
        }

        foreach (var optionId in request.OptionIds)
        {
            if (!poll.Options.Any(o => o.Id == optionId))
            {
                throw new NotFoundException($"گزینه با شناسه {optionId} برای این نظرسنجی معتبر نیست.");
            }
        }

        foreach (var optionId in request.OptionIds)
        {
            var vote = new Vote
            {
                PollOptionId = optionId,
                UserId = request.UserId
            };
            await _context.Votes.AddAsync(vote, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}