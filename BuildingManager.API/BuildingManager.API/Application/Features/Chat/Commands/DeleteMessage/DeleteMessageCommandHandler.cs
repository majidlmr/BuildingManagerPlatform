using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Interfaces;
using BuildingManager.API.Hubs;
using FluentValidation; // 👈 using جدید
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Chat.Commands.DeleteMessage;

public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHubContext<ChatHub> _hubContext;
    private const int DeletionTimeLimitInMinutes = 5;

    public DeleteMessageCommandHandler(IApplicationDbContext context, IUnitOfWork unitOfWork, IHubContext<ChatHub> hubContext)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _hubContext = hubContext;
    }

    public async Task Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        var message = await _context.Messages
            .Include(m => m.Conversation)
                .ThenInclude(c => c.Participants)
            .FirstOrDefaultAsync(m => m.PublicId == request.MessagePublicId, cancellationToken);

        if (message == null)
        {
            throw new NotFoundException("پیام مورد نظر یافت نشد.");
        }

        if (message.SenderUserId != request.RequestingUserId)
        {
            // استفاده از خطای دسترسی استاندارد
            throw new ForbiddenAccessException("شما فقط می‌توانید پیام‌های خود را حذف کنید.");
        }

        if (message.IsDeleted)
        {
            return;
        }

        var timeSinceSent = DateTime.UtcNow - message.SentAt;
        if (timeSinceSent.TotalMinutes > DeletionTimeLimitInMinutes)
        {
            // استفاده از خطای اعتبارسنجی برای قوانین کسب‌وکار
            throw new ValidationException($"شما فقط تا {DeletionTimeLimitInMinutes} دقیقه پس از ارسال می‌توانید پیام خود را حذف کنید.");
        }

        message.IsDeleted = true;
        message.DeletedAt = DateTime.UtcNow;
        message.Content = "این پیام حذف شده است.";

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var participantIds = message.Conversation.Participants.Select(p => p.UserId.ToString()).ToList();

        await _hubContext.Clients.Groups(participantIds)
            .SendAsync("MessageDeleted", new
            {
                conversationId = message.Conversation.PublicId,
                messageId = message.PublicId
            }, cancellationToken);
    }
}