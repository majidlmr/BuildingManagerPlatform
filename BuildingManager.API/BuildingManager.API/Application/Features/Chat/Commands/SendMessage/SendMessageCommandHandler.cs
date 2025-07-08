using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Interfaces;
using BuildingManager.API.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Chat.Commands.SendMessage;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHubContext<ChatHub> _hubContext;

    public SendMessageCommandHandler(IApplicationDbContext context, IUnitOfWork unitOfWork, IHubContext<ChatHub> hubContext)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _hubContext = hubContext;
    }

    public async Task Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _context.Conversations
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c => c.PublicId == request.ConversationPublicId, cancellationToken);

        if (conversation == null)
        {
            throw new NotFoundException("گفتگو یافت نشد.");
        }

        // ✅ TODO تکمیل شد: بررسی اینکه آیا کاربر ارسال‌کننده، عضو این گفتگو است
        var isParticipant = conversation.Participants.Any(p => p.UserId == request.SenderUserId);
        if (!isParticipant)
        {
            throw new ForbiddenAccessException("شما اجازه ارسال پیام در این گفتگو را ندارید.");
        }

        var message = new Message
        {
            ConversationId = conversation.Id,
            SenderUserId = request.SenderUserId,
            Content = request.Content,
            IsAnonymous = request.IsAnonymous,
            SentAt = System.DateTime.UtcNow
        };

        await _context.Messages.AddAsync(message, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var sender = await _context.Users.FindAsync(request.SenderUserId);
        var senderName = (request.IsAnonymous || sender == null) ? "کاربر ناشناس" : sender.FullName;

        var participantIds = conversation.Participants.Select(p => p.UserId.ToString()).ToList();

        await _hubContext.Clients.Groups(participantIds)
            .SendAsync("ReceiveMessage", new
            {
                conversationId = conversation.PublicId,
                messageId = message.PublicId, // 👈 شناسه پیام جدید را هم ارسال می‌کنیم
                sender = senderName,
                content = message.Content,
                sentAt = message.SentAt,
                isSentByCurrentUser = false // این مقدار در کلاینت باید برای هر کاربر مشخص شود
            }, cancellationToken);
    }
}