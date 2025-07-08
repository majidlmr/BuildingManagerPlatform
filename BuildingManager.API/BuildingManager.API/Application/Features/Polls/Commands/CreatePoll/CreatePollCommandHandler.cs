using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Polls.Commands.CreatePoll;

/// <summary>
/// پردازشگر دستور ایجاد یک نظرسنجی جدید.
/// </summary>
public class CreatePollCommandHandler : IRequestHandler<CreatePollCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    public CreatePollCommandHandler(IApplicationDbContext context, IUnitOfWork unitOfWork, IAuthorizationService authorizationService)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// منطق اصلی ایجاد یک نظرسنجی را به همراه گزینه‌های آن مدیریت می‌کند.
    /// </summary>
    public async Task<int> Handle(CreatePollCommand request, CancellationToken cancellationToken)
    {
        // ✅ بررسی دسترسی با استفاده از سیستم جدید مبتنی بر مجوز "Poll.Create"
        var canCreate = await _authorizationService.HasPermissionAsync(request.CreatedByUserId, request.BuildingId, "Poll.Create", cancellationToken);
        if (!canCreate)
        {
            throw new ForbiddenAccessException("شما اجازه ایجاد نظرسنجی برای این ساختمان را ندارید.");
        }

        // ایجاد موجودیت اصلی نظرسنجی
        var poll = new Poll
        {
            BuildingId = request.BuildingId,
            Question = request.Question,
            IsMultipleChoice = request.IsMultipleChoice,
            EndDate = request.EndDate?.ToUniversalTime(),
            CreatedByUserId = request.CreatedByUserId,
            IsActive = true
        };

        // ایجاد و افزودن گزینه‌های نظرسنجی
        foreach (var optionText in request.Options)
        {
            poll.Options.Add(new PollOption { Text = optionText });
        }

        await _context.Polls.AddAsync(poll, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return poll.Id;
    }
}