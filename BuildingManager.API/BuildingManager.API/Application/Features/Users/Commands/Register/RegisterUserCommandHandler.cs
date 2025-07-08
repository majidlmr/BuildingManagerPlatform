using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Events;
using BuildingManager.API.Domain.Interfaces;
using FluentValidation;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Users.Commands.Register;

/// <summary>
/// پردازشگر دستور ثبت‌نام یک کاربر جدید در سیستم.
/// در معماری جدید، این بخش فقط کاربر را ایجاد کرده و سپس یک رویداد
/// UserRegisteredEvent را برای اطلاع سایر بخش‌های برنامه، منتشر می‌کند.
/// </summary>
public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, int>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublisher _publisher; // برای انتشار رویدادها

    public RegisterUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IPublisher publisher)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _publisher = publisher;
    }

    /// <summary>
    /// منطق اصلی ثبت کاربر و انتشار رویداد را مدیریت می‌کند.
    /// </summary>
    public async Task<int> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // گام ۱: بررسی اینکه آیا کاربری با این شماره موبایل از قبل وجود دارد یا خیر
        var userExists = await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber);
        if (userExists != null)
        {
            throw new ValidationException($"کاربری با شماره موبایل '{request.PhoneNumber}' قبلا ثبت نام کرده است.");
        }

        // گام ۲: هش کردن رمز عبور برای ذخیره‌سازی امن
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // گام ۳: ایجاد موجودیت کاربر جدید
        // توجه کنید که در این مرحله هیچ نقشی به کاربر تخصیص داده نمی‌شود.
        var user = new User
        {
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            PasswordHash = passwordHash,
            PhoneNumberConfirmed = true // فرض بر این است که شماره موبایل در فرآیند ثبت‌نام تایید می‌شود.
                                        // در سناریوی واقعی، این مقدار باید false باشد تا کاربر با OTP آن را تایید کند.
        };

        // گام ۴: ذخیره کاربر جدید در دیتابیس
        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // گام ۵ (مهم): انتشار رویداد ثبت‌نام کاربر
        // پس از ذخیره موفقیت‌آمیز، به تمام سیستم اعلام می‌کنیم که یک کاربر جدید ثبت‌نام کرده است.
        // سایر بخش‌ها (مانند ارسال ایمیل خوشامدگویی) که به این رویداد گوش می‌دهند، فعال خواهند شد.
        await _publisher.Publish(new UserRegisteredEvent(user), cancellationToken);

        // گام ۶: بازگرداندن شناسه کاربر جدید
        return user.Id;
    }
}