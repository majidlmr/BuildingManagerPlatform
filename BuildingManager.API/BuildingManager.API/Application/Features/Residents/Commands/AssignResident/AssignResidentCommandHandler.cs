using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Events;
using BuildingManager.API.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Residents.Commands.AssignResident;

/// <summary>
/// پردازشگر دستور تخصیص یک ساکن جدید به یک واحد.
/// این نسخه مسئولیت‌های زیر را بر عهده دارد:
/// 1. بررسی دسترسی مدیر برای دعوت عضو جدید.
/// 2. ایجاد کاربر جدید برای ساکن (در صورت عدم وجود) به روشی امن.
/// 3. غیرفعال کردن تخصیص‌های قبلی واحد.
/// 4. ثبت تخصیص جدید ساکن به واحد.
/// 5. انتشار رویداد 'ResidentAssignedEvent' برای انجام عملیات‌های بعدی مانند تخصیص نقش.
/// </summary>
public class AssignResidentCommandHandler : IRequestHandler<AssignResidentCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;
    private readonly IPublisher _publisher;

    public AssignResidentCommandHandler(
        IApplicationDbContext context,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IAuthorizationService authorizationService,
        IPublisher publisher)
    {
        _context = context;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
        _publisher = publisher;
    }

    public async Task<int> Handle(AssignResidentCommand request, CancellationToken cancellationToken)
    {
        // گام ۱: پیدا کردن واحد و شناسه ساختمان برای بررسی دسترسی
        var unit = await _context.Units
            .AsNoTracking() // فقط برای خواندن اطلاعات استفاده می‌شود
            .FirstOrDefaultAsync(u => u.Id == request.UnitId, cancellationToken);

        if (unit == null)
        {
            throw new NotFoundException("واحد مورد نظر یافت نشد.");
        }

        // گام ۲: بررسی دسترسی با استفاده از سیستم جدید مبتنی بر مجوز "Member.Invite"
        var canInvite = await _authorizationService.HasPermissionAsync(request.RequestingUserId, unit.BuildingId, "Member.Invite", cancellationToken);
        if (!canInvite)
        {
            throw new ForbiddenAccessException("شما اجازه تخصیص ساکن به این واحد را ندارید.");
        }

        // گام ۳: بررسی وجود کاربر ساکن و ایجاد آن در صورت نیاز (فرآیند امن)
        var residentUser = await _userRepository.GetByPhoneNumberAsync(request.ResidentPhoneNumber);
        if (residentUser == null)
        {
            // ایجاد کاربر با یک رمز عبور غیرقابل استفاده برای افزایش امنیت
            var unuseablePasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString());
            residentUser = new User
            {
                FullName = request.ResidentFullName,
                PhoneNumber = request.ResidentPhoneNumber,
                PasswordHash = unuseablePasswordHash,
                PhoneNumberConfirmed = false // کاربر باید هویت خود را با OTP تایید کند
            };
            await _userRepository.AddAsync(residentUser);

            // TODO: در اینجا کد ارسال OTP به شماره موبایل کاربر قرار می‌گیرد
            var otpCode = new Random().Next(100000, 999999).ToString();
            Console.WriteLine($"[SECURITY-INFO] OTP code for {residentUser.PhoneNumber} is: {otpCode}. This should be sent via SMS.");
        }

        // گام ۴: غیرفعال کردن تخصیص‌های قبلی و فعال این واحد
        var previousAssignments = await _context.ResidentAssignments
            .Where(ra => ra.UnitId == request.UnitId && ra.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var assignment in previousAssignments)
        {
            assignment.IsActive = false;
            if (assignment.EndDate == null)
            {
                assignment.EndDate = request.StartDate.ToUniversalTime().AddDays(-1);
            }
        }

        // گام ۵: ایجاد تخصیص جدید برای ساکن
        var newAssignment = new ResidentAssignment
        {
            UnitId = request.UnitId,
            ResidentUserId = residentUser.Id,
            StartDate = request.StartDate.ToUniversalTime(),
            EndDate = request.EndDate?.ToUniversalTime(),
            IsActive = true
        };

        await _context.ResidentAssignments.AddAsync(newAssignment, cancellationToken);

        // گام ۶: ذخیره تغییرات در دیتابیس
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // گام ۷ (مهم): انتشار رویداد "ساکن تخصیص داده شد"
        // این کار به سایر بخش‌های سیستم (مانند ماژول تخصیص نقش) اجازه می‌دهد تا به این اتفاق واکنش نشان دهند.
        await _publisher.Publish(new ResidentAssignedEvent(residentUser.Id, unit.BuildingId), cancellationToken);

        return newAssignment.Id;
    }
}