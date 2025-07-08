using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Events;
using BuildingManager.API.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Buildings.Commands.Create;

/// <summary>
/// پردازشگر دستور ایجاد یک ساختمان (یا بلوک) جدید.
/// این نسخه مسئولیت‌های زیر را بر عهده دارد:
/// 1. بررسی دسترسی برای ایجاد بلوک در یک مجتمع.
/// 2. ایجاد نقش پیش‌فرض "Owner".
/// 3. تخصیص تمام دسترسی‌ها به نقش "Owner".
/// 4. ثبت کاربر سازنده به عنوان اولین عضو این نقش.
/// 5. انتشار رویداد 'BuildingCreatedEvent' پس از اتمام کار.
/// </summary>
public class CreateBuildingCommandHandler : IRequestHandler<CreateBuildingCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;
    private readonly IPublisher _publisher;

    public CreateBuildingCommandHandler(
        IApplicationDbContext context,
        IUnitOfWork unitOfWork,
        IAuthorizationService authorizationService,
        IPublisher publisher) // ✅ تزریق ناشر رویداد
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
        _publisher = publisher; // ✅ مقداردهی ناشر
    }

    public async Task<int> Handle(CreateBuildingCommand request, CancellationToken cancellationToken)
    {
        // گام ۱: بررسی دسترسی برای ایجاد بلوک در یک مجتمع (در صورت وجود)
        if (request.ParentBuildingId.HasValue)
        {
            // اگر کاربر قصد دارد یک بلوک بسازد، باید دسترسی مدیریت بر روی مجتمع والد را داشته باشد.
            var canManageParent = await _authorizationService.HasPermissionAsync(request.OwnerUserId, request.ParentBuildingId.Value, "Building.Update", cancellationToken);
            if (!canManageParent)
            {
                throw new ForbiddenAccessException("شما اجازه افزودن بلوک جدید به این مجتمع را ندارید.");
            }
        }

        // گام ۲: ایجاد موجودیت ساختمان با لحاظ کردن والد احتمالی
        var building = new Building
        {
            Name = request.Name,
            BuildingType = request.BuildingType,
            Address = request.Address,
            NumberOfFloors = request.NumberOfFloors,
            TotalUnits = request.TotalUnits,
            ConstructionYear = request.ConstructionYear,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Amenities = request.Amenities,
            ParentBuildingId = request.ParentBuildingId // مقداردهی شناسه والد
        };

        // گام ۳: ایجاد نقش اصلی "Owner" برای این ساختمان/بلوک جدید
        var ownerRole = new Role
        {
            Building = building,
            Name = "Owner",
        };

        // گام ۴: تخصیص تمام دسترسی‌های موجود در سیستم به نقش "Owner"
        var allPermissions = await SeedAndGetAllPermissionsAsync(cancellationToken);
        foreach (var permission in allPermissions)
        {
            ownerRole.Permissions.Add(new RolePermission { Permission = permission });
        }

        // گام ۵: ثبت کاربر سازنده به عنوان مدیر در جدول ManagerAssignments
        var managerAssignment = new ManagerAssignment
        {
            UserId = request.OwnerUserId,
            Building = building,
            Role = "Owner"
        };

        // گام ۶: تخصیص نقش "Owner" به کاربر سازنده در جدول UserRoles
        var userRoleAssignment = new UserRole
        {
            UserId = request.OwnerUserId,
            Role = ownerRole
        };

        // گام ۷: افزودن تمام موجودیت‌های جدید به DbContext
        await _context.Buildings.AddAsync(building, cancellationToken);
        await _context.Roles.AddAsync(ownerRole, cancellationToken);
        await _context.UserRoles.AddAsync(userRoleAssignment, cancellationToken);
        await _context.ManagerAssignments.AddAsync(managerAssignment, cancellationToken);

        // گام ۸: ذخیره تمام تغییرات در یک تراکنش واحد در دیتابیس
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // گام ۹ (نهایی): انتشار رویداد "ساختمان ایجاد شد" برای اطلاع سایر ماژول‌ها
        await _publisher.Publish(new BuildingCreatedEvent(building), cancellationToken);

        return building.Id;
    }

    /// <summary>
    /// یک متد کمکی برای ایجاد/واکشی تمام دسترسی‌های ممکن در سیستم.
    /// در اولین اجرا، این متد دسترسی‌ها را در جدول Permissions ایجاد می‌کند (Seed می‌کند).
    /// </summary>
    private async Task<List<Permission>> SeedAndGetAllPermissionsAsync(CancellationToken cancellationToken)
    {
        var permissionNames = new List<string>
        {
            "Building.Update", "Building.Delete",
            "Unit.Create", "Unit.Update", "Unit.Delete",
            "Member.Read", "Member.Invite", "Member.Remove",
            "Role.Create", "Role.Update", "Role.Delete", "Role.Assign",
            "Ticket.Read", "Ticket.UpdateStatus",
            "Billing.CreateCycle", "Billing.Read", "Financials.ReadSummary", "Expense.Create", "Revenue.Create",
            "Announcement.Create", "Rule.Create", "Poll.Create"
        }.Distinct().ToList();

        var existingPermissions = await _context.Permissions
            .Where(p => permissionNames.Contains(p.Name))
            .ToListAsync(cancellationToken);

        var newPermissionNames = permissionNames.Except(existingPermissions.Select(p => p.Name));

        if (newPermissionNames.Any())
        {
            foreach (var name in newPermissionNames)
            {
                var permission = new Permission { Name = name, Description = $"Allows to {name.Replace('.', ' ')}" };
                await _context.Permissions.AddAsync(permission, cancellationToken);
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return await _context.Permissions.ToListAsync(cancellationToken);
    }
}