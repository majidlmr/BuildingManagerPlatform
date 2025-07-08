using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Events;
using BuildingManager.API.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Buildings.EventHandlers;

/// <summary>
/// این Handler به رویداد BuildingCreatedEvent گوش می‌دهد و به صورت خودکار
/// یک گفتگوی چت عمومی برای ساختمان جدید ایجاد می‌کند.
/// </summary>
public class CreateDefaultChatRoomEventHandler : INotificationHandler<BuildingCreatedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDefaultChatRoomEventHandler(IApplicationDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(BuildingCreatedEvent notification, CancellationToken cancellationToken)
    {
        var building = notification.Building;

        // گام ۱: ایجاد یک گفتگوی جدید از نوع گروهی
        var conversation = new Conversation
        {
            Type = "Group",
            Name = $"چت عمومی ساختمان {building.Name}",
            BuildingId = building.Id,
            ImageUrl = null // می‌توان یک تصویر پیش‌فرض قرار داد
        };

        // گام ۲: پیدا کردن تمام کاربرانی که در این ساختمان نقش دارند (مدیران و ساکنین)
        var memberIds = await _context.UserRoles
            .Where(ur => ur.Role.BuildingId == building.Id)
            .Select(ur => ur.UserId)
            .Distinct()
            .ToListAsync(cancellationToken);

        // گام ۳: افزودن تمام اعضا به گفتگوی جدید
        foreach (var userId in memberIds)
        {
            conversation.Participants.Add(new Participant { UserId = userId });
        }

        await _context.Conversations.AddAsync(conversation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}