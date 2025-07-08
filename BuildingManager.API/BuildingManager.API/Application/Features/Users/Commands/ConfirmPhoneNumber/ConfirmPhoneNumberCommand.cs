using MediatR;

namespace BuildingManager.API.Application.Features.Users.Commands.ConfirmPhoneNumber;

/// <summary>
/// دستوری برای تایید شماره موبایل کاربر با استفاده از کد OTP و تنظیم رمز عبور جدید.
/// این دستور پس از ثبت‌نام اولیه ساکن توسط مدیر، توسط خود ساکن استفاده می‌شود.
/// </summary>
/// <param name="PhoneNumber">شماره موبایلی که باید تایید شود.</param>
/// <param name="OtpCode">کد یکبار مصرف (OTP) دریافت شده از طریق پیامک.</param>
/// <param name="NewPassword">رمز عبور جدید و دائمی کاربر.</param>
public record ConfirmPhoneNumberCommand(
    string PhoneNumber,
    string OtpCode,
    string NewPassword
) : IRequest; // این دستور نتیجه خاصی برنمی‌گرداند