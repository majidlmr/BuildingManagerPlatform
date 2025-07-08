// File: Application/Features/Chat/Commands/StartConversation/StartConversationCommandHandler.cs
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuildingManager.API.Application.Common.Exceptions;

namespace BuildingManager.API.Application.Features.Chat.Commands.StartConversation;

/// <summary>
/// پردازشگر دستور شروع یا یافتن یک گفتگوی دو نفره.
/// </summary>
public class StartConversationCommandHandler : IRequestHandler<StartConversationCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public StartConversationCommandHandler(IApplicationDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(StartConversationCommand request, CancellationToken cancellationToken)
    {
        // گام ۱: بررسی اینکه آیا یک گفتگوی دو نفره بین این دو کاربر از قبل وجود دارد یا خیر.
        // یک گفتگو، دو نفره است اگر دقیقاً دو شرکت‌کننده با شناسه‌های مشخص شده داشته باشد.
        var existingConversation = await _context.Conversations
            .AsNoTracking()
            .Where(c => c.Type == "Direct" &&
                        c.Participants.Count == 2 &&
                        c.Participants.Any(p => p.UserId == request.InitiatorUserId) &&
                        c.Participants.Any(p => p.UserId == request.OtherUserId))
            .Select(c => c.PublicId)
            .FirstOrDefaultAsync(cancellationToken);

        // اگر گفتگو از قبل وجود داشت، شناسه آن را برمی‌گردانیم و عملیات پایان می‌یابد.
        if (existingConversation != Guid.Empty)
        {
            return existingConversation;
        }

        // گام ۲: اگر گفتگویی وجود نداشت، یک گفتگوی جدید ایجاد می‌کنیم.
        // ابتدا اطلاعات هر دو کاربر را برای ذخیره نام و عکس در گفتگو واکشی می‌کنیم.
        var initiatorUser = await _context.Users.FindAsync(request.InitiatorUserId);
        var otherUser = await _context.Users.FindAsync(request.OtherUserId);

        if (initiatorUser == null || otherUser == null)
        {
            throw new NotFoundException("یکی از کاربران یافت نشد.");
        }

        // ایجاد موجودیت اصلی گفتگو
        var conversation = new Conversation
        {
            Type = "Direct",
            // برای بهینه‌سازی، نام و عکس گفتگو را از دید هر کاربر مشخص می‌کنیم
            // (این بخش در آینده می‌تواند پیچیده‌تر شود)
            Name = otherUser.FullName, // نام گفتگو از دید شروع‌کننده، نام طرف مقابل است
            ImageUrl = otherUser.ProfilePictureUrl,
        };

        // افزودن هر دو کاربر به عنوان شرکت‌کننده
        conversation.Participants.Add(new Participant { UserId = initiatorUser.Id });
        conversation.Participants.Add(new Participant { UserId = otherUser.Id });

        // افزودن گفتگو به دیتابیس
        await _context.Conversations.AddAsync(conversation, cancellationToken);

        // ذخیره تمام تغییرات در یک تراکنش
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return conversation.PublicId;
    }
}