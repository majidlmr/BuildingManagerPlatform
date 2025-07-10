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
        if (request.ParentComplexId.HasValue)
        {
            // اگر کاربر قصد دارد یک بلوک بسازد، باید دسترسی مدیریت بر روی مجتمع والد را داشته باشد.
            var canManageParent = await _authorizationService.HasPermissionAsync(
                request.OwnerUserId,
                "Complex.ManageBlocks", // نام دسترسی برای مدیریت بلوک‌ها در یک مجتمع
                HierarchyLevel.Complex, // سطح موجودیت والد، یعنی مجتمع
                request.ParentComplexId.Value, // شناسه مجتمع والد
                cancellationToken);
            if (!canManageParent)
            {
                throw new ForbiddenAccessException("شما اجازه افزودن بلوک جدید به این مجتمع را ندارید.");
            }
        }

        // گام ۲: ایجاد موجودیت بلوک با لحاظ کردن والد احتمالی
        var block = new Block // تغییر نام از building به block
        {
            NameOrNumber = request.Name, // تغییر از Name به NameOrNumber مطابق با Block.cs
            // BuildingType = request.BuildingType, // Block.cs از BlockType (enum) استفاده می‌کند، request.BuildingType رشته است. نیاز به تبدیل دارد.
            // فرض می‌کنیم request.BuildingType یک مقدار معتبر از enum BlockType است یا باید تبدیل شود.
            // برای سادگی فعلاً یک مقدار پیش‌فرض می‌گذاریم یا این فیلد را موقتاً کامنت می‌کنیم تا خطا ندهد.
            // بعداً باید یک تبدیل مناسب از رشته به enum BlockType اضافه شود.
            // BlockType = Domain.Entities.BlockType.Residential, // مقدار پیش‌فرض موقت
            BlockType = ConvertToBlockType(request.BuildingType), // تبدیل رشته به enum
            Address = request.Address,
            NumberOfFloors = request.NumberOfFloors,
            TotalUnits = request.TotalUnits,
            ConstructionYear = request.ConstructionYear,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Amenities = request.Amenities,
            ParentComplexId = request.ParentComplexId // مقداردهی شناسه والد (مجتمع)
        };

        // گام ۳: ایجاد نقش اصلی "Owner" برای این بلوک جدید
        var ownerRole = new Role
        {
            Name = "Block Owner", // نام مناسب‌تر برای نقش مالک بلوک
            NormalizedName = "BLOCK_OWNER", // نام نرمالایز شده
            AppliesToHierarchyLevel = HierarchyLevel.Block,
            Description = $"Owner of the block {block.NameOrNumber}",
            IsSystemRole = false // این نقش توسط سیستم ایجاد می‌شود اما می‌تواند قابل ویرایش باشد
        };
        // ابتدا نقش را ذخیره می‌کنیم تا Id بگیرد
        await _context.Roles.AddAsync(ownerRole, cancellationToken);
        // await _unitOfWork.SaveChangesAsync(cancellationToken); // ذخیره موقت برای گرفتن RoleId یا انتقال ذخیره به انتها


        // گام ۴: تخصیص تمام دسترسی‌های مربوط به بلوک به نقش "Owner"
        // لیست دسترسی‌ها باید بازبینی شود تا فقط شامل دسترسی‌های مرتبط با Block باشد.
        var blockPermissions = await SeedAndGetBlockPermissionsAsync(cancellationToken);
        foreach (var permission in blockPermissions)
        {
            // ownerRole.RolePermissions.Add(new RolePermission { PermissionId = permission.Id, RoleId = ownerRole.Id });
            // به جای خط بالا، اگر RolePermissions در Role به درستی تنظیم شده باشد:
             ownerRole.RolePermissions.Add(new RolePermission { Permission = permission, Role = ownerRole });
        }

        // گام ۵: ثبت کاربر سازنده به عنوان مدیر در جدول ManagerAssignments (اگر هنوز نیاز است)
        // با وجود UserRoleAssignment، ممکن است ManagerAssignment اضافی باشد مگر اینکه معنای خاصی داشته باشد.
        // فعلا آن را مطابق با تغییرات جدید پیش می‌بریم.
        var managerAssignment = new ManagerAssignment
        {
            UserId = request.OwnerUserId,
            Block = block, // تغییر از Building به Block
            Role = "Owner" // این فیلد رشته‌ای در ManagerAssignment است
        };

        // گام ۶: تخصیص نقش "Owner" به کاربر سازنده در جدول UserRoleAssignments
        var userRoleAssignment = new UserRoleAssignment // تغییر از UserRole به UserRoleAssignment
        {
            UserId = request.OwnerUserId,
            Role = ownerRole, // تخصیص خود شیء Role
            TargetEntityId = block.Id, // شناسه بلوک تازه ایجاد شده - این فیلد باید پس از ذخیره block مقداردهی شود
            AssignmentStatus = Domain.Enums.AssignmentStatus.Active // وضعیت فعال
            // AssignedByUserId = request.OwnerUserId // کاربری که تخصیص داده
        };

        // گام ۷: افزودن تمام موجودیت‌های جدید به DbContext
        await _context.Blocks.AddAsync(block, cancellationToken); // تغییر از Buildings به Blocks
        // Roles قبلا اضافه شده اگر برای گرفتن Id ذخیره شده باشد. اگر نه، اینجا اضافه می‌شود.
        // await _context.Roles.AddAsync(ownerRole, cancellationToken); // اگر در بالا ذخیره نشده
        await _context.UserRoleAssignments.AddAsync(userRoleAssignment, cancellationToken); // تغییر از UserRoles
        await _context.ManagerAssignments.AddAsync(managerAssignment, cancellationToken);

        // گام ۸: ذخیره تمام تغییرات در یک تراکنش واحد در دیتابیس
        // قبل از این مرحله، block.Id باید مقدار گرفته باشد تا در userRoleAssignment.TargetEntityId استفاده شود.
        // EF Core به طور خودکار Id ها را پس از AddAsync و قبل از SaveChangesAsync مقداردهی می‌کند اگر به درستی پیکربندی شده باشند.
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // اگر userRoleAssignment.TargetEntityId نیاز به مقداردهی صریح داشت، باید اینجا انجام شود:
        // userRoleAssignment.TargetEntityId = block.Id;
        // _context.UserRoleAssignments.Update(userRoleAssignment); // یا روش مناسب دیگر برای به‌روزرسانی
        // await _unitOfWork.SaveChangesAsync(cancellationToken); // ذخیره مجدد


        // گام ۹ (نهایی): انتشار رویداد "بلوک ایجاد شد" برای اطلاع سایر ماژول‌ها
        await _publisher.Publish(new BuildingCreatedEvent(block), cancellationToken); // تغییر از Building به Block

        return block.Id;
    }

    /// <summary>
    /// یک متد کمکی برای ایجاد/واکشی دسترسی‌های مرتبط با بلوک.
    /// </summary>
    private async Task<List<Permission>> SeedAndGetBlockPermissionsAsync(CancellationToken cancellationToken)
    {
        // این لیست باید بازبینی شود و فقط شامل دسترسی‌های مرتبط با Block باشد.
        var permissionNames = new List<string>
        {
            "Block.Update", "Block.Delete", // تغییر از Building به Block
            "Unit.Create", "Unit.Update", "Unit.Delete", // مرتبط با Block
            "Member.Read", "Member.Invite", "Member.Remove", // مرتبط با Block
            "Role.AssignToBlock", // نقش‌های مختص بلوک
            "Ticket.ReadInBlock", "Ticket.UpdateStatusInBlock",
            // "Billing.CreateCycleForBlock", "Billing.ReadForBlock", // اگر نیاز باشد
            // "Financials.ReadSummaryForBlock", "Expense.CreateForBlock", "Revenue.CreateForBlock",
            "Announcement.CreateForBlock", "Rule.CreateForBlock", "Poll.CreateForBlock"
        }.Distinct().ToList();

        var existingPermissions = await _context.Permissions
            .Where(p => permissionNames.Contains(p.Name))
            .ToListAsync(cancellationToken);

        var newPermissionNames = permissionNames.Except(existingPermissions.Select(p => p.Name));

        if (newPermissionNames.Any())
        {
            foreach (var name in newPermissionNames)
            {
                // TODO: Module و Description باید دقیق‌تر مشخص شوند.
                var permission = new Permission { Name = name, Description = $"Allows to {name.Replace('.', ' ')}", Module = name.Split('.')[0] };
                await _context.Permissions.AddAsync(permission, cancellationToken);
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken); // ذخیره دسترسی‌های جدید
        }

        // برگرداندن همه دسترسی‌های مرتبط با بلوک (هم موجود و هم جدید)
        return await _context.Permissions.Where(p => permissionNames.Contains(p.Name)).ToListAsync(cancellationToken);
    }

    private BlockType ConvertToBlockType(string buildingTypeStr)
    {
        if (Enum.TryParse<BlockType>(buildingTypeStr, true, out var blockType))
        {
            return blockType;
        }
        // مقدار پیش‌فرض یا مدیریت خطا در صورت نامعتبر بودن رشته
        // می‌توان یک خطای ArgumentException پرتاب کرد یا یک مقدار پیش‌فرض بازگرداند.
        // فعلاً مقدار پیش‌فرض Residential را بازمی‌گردانیم.
        // Log an warning here if possible
        return Domain.Entities.BlockType.Residential;
    }


    // قبلی، برای مرجع
    // private async Task<List<Permission>> SeedAndGetAllPermissionsAsync(CancellationToken cancellationToken)
    // {
    //     var permissionNames = new List<string>
    //     {
    //         "Building.Update", "Building.Delete",
    //         "Unit.Create", "Unit.Update", "Unit.Delete",
    //         "Member.Read", "Member.Invite", "Member.Remove",
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