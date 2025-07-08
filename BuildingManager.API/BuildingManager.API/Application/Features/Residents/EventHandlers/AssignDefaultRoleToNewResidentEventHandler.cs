using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Events;
using BuildingManager.API.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Residents.EventHandlers;

/// <summary>
/// این Handler به رویداد ResidentAssignedEvent گوش می‌دهد و در صورت نیاز،
/// نقش پیش‌فرض "Resident" را به کاربر جدید تخصیص می‌دهد.
/// </summary>
public class AssignDefaultRoleToNewResidentEventHandler : INotificationHandler<ResidentAssignedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public AssignDefaultRoleToNewResidentEventHandler(IApplicationDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ResidentAssignedEvent notification, CancellationToken cancellationToken)
    {
        // گام ۱: بررسی اینکه آیا کاربر از قبل نقشی در این ساختمان دارد یا خیر
        var alreadyHasRole = await _context.UserRoles
            .AnyAsync(ur => ur.UserId == notification.UserId && ur.Role.BuildingId == notification.BuildingId, cancellationToken);

        // اگر کاربر از قبل نقشی دارد، عملیات جدیدی لازم نیست
        if (alreadyHasRole)
        {
            return;
        }

        // گام ۲: پیدا کردن یا ایجاد نقش پیش‌فرض "Resident" برای این ساختمان
        var residentRole = await _context.Roles
            .FirstOrDefaultAsync(r => r.BuildingId == notification.BuildingId && r.Name == "Resident", cancellationToken);

        if (residentRole == null)
        {
            // اگر نقش "Resident" وجود نداشت، آن را ایجاد می‌کنیم
            residentRole = new Role { BuildingId = notification.BuildingId, Name = "Resident" };
            await _context.Roles.AddAsync(residentRole, cancellationToken);
            // نیازی به ذخیره فوری نیست، در تراکنش نهایی ذخیره می‌شود
        }

        // گام ۳: تخصیص نقش "Resident" به کاربر
        var newUserRole = new UserRole
        {
            UserId = notification.UserId,
            Role = residentRole
        };

        await _context.UserRoles.AddAsync(newUserRole, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}