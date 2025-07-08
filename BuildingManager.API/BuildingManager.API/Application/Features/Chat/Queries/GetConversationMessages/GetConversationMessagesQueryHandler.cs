// File: Application/Features/Chat/Queries/GetConversationMessages/GetConversationMessagesQueryHandler.cs
using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Application.Features.Chat.Queries.GetMyConversations; // اطمینان از وجود این using
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Chat.Queries.GetConversationMessages;

/// <summary>
/// پردازشگر دستور دریافت تاریخچه پیام‌های یک گفتگوی مشخص.
/// </summary>
public class GetConversationMessagesQueryHandler : IRequestHandler<GetConversationMessagesQuery, List<MessageDto>>
{
    private readonly IApplicationDbContext _context;

    public GetConversationMessagesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MessageDto>> Handle(GetConversationMessagesQuery request, CancellationToken cancellationToken)
    {
        // گام ۱: بررسی دسترسی - آیا کاربر درخواست‌دهنده عضوی از این گفتگو است؟
        var isParticipant = await _context.Participants
            .AnyAsync(p => p.Conversation.PublicId == request.ConversationPublicId && p.UserId == request.RequestingUserId, cancellationToken);

        if (!isParticipant)
        {
            throw new ForbiddenAccessException("شما اجازه دسترسی به پیام‌های این گفتگو را ندارید.");
        }

        // گام ۲: واکشی پیام‌ها از دیتابیس به همراه اطلاعات فرستنده
        var messages = await _context.Messages
            .Where(m => m.Conversation.PublicId == request.ConversationPublicId)
            .OrderBy(m => m.SentAt) // مرتب‌سازی پیام‌ها بر اساس زمان ارسال
            .Include(m => m.Sender) // برای دسترسی به نام فرستنده
            .AsNoTracking()
            .Select(m => new MessageDto(
                // 🚀 تغییر اصلی: به جای m.Id از شناسه عمومی جدید یعنی m.PublicId استفاده می‌کنیم.
                m.PublicId,
                m.IsDeleted ? "این پیام حذف شده است." : m.Content,
                m.SentAt,
                m.IsAnonymous ? "کاربر ناشناس" : m.Sender.FullName,
                m.SenderUserId == request.RequestingUserId,
                m.IsDeleted
            ))
            .ToListAsync(cancellationToken);

        return messages ?? new List<MessageDto>();
    }
}