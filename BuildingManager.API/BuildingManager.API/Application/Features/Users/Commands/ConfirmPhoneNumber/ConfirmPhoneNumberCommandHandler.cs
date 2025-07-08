using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Users.Commands.ConfirmPhoneNumber;

/// <summary>
/// پردازشگر دستور تایید شماره موبایل و تنظیم رمز عبور.
/// </summary>
public class ConfirmPhoneNumberCommandHandler : IRequestHandler<ConfirmPhoneNumberCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmPhoneNumberCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ConfirmPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        // گام ۱: کاربر را بر اساس شماره موبایل پیدا می‌کنیم
        var user = await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber);
        if (user == null)
        {
            throw new NotFoundException("کاربری با این شماره موبایل یافت نشد.");
        }

        if (user.PhoneNumberConfirmed)
        {
            // اگر کاربر قبلاً تایید شده، عملیات را متوقف می‌کنیم
            // می‌توان یک پیام مناسب‌تر نیز برگرداند
            return;
        }

        // گام ۲: بررسی اعتبار کد OTP
        // ⚠️ نکته مهم: در یک پروژه واقعی، این بخش باید به صورت امن پیاده‌سازی شود.
        // کد OTP که در مرحله قبل تولید شده، باید به همراه شماره موبایل و یک تاریخ انقضا (مثلا ۲ دقیقه)
        // در یک حافظه موقت و سریع مانند Redis یا MemoryCache ذخیره شود.
        // در این بخش، کد ارسال شده توسط کاربر با کد ذخیره شده در Cache مقایسه می‌شود.
        // ما در اینجا صرفاً آن را شبیه‌سازی می‌کنیم و هر کد ۶ رقمی را معتبر می‌دانیم.
        if (request.OtpCode.Length != 6) // این یک بررسی ساده و موقتی است
        {
            throw new Exception("کد تایید نامعتبر است.");
        }

        // گام ۳: هش کردن و به‌روزرسانی رمز عبور جدید
        var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.PasswordHash = newPasswordHash;
        user.PhoneNumberConfirmed = true; // شماره موبایل کاربر اکنون تایید شده است
        user.UpdatedAt = DateTime.UtcNow;

        // گام ۴: ذخیره تغییرات در دیتابیس
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}