// File: Application/Common/Exceptions/NotFoundException.cs
using System;

namespace BuildingManager.API.Application.Common.Exceptions;

/// <summary>
/// خطای سفارشی برای مواردی که یک منبع خاص یافت نمی‌شود.
/// این خطا توسط ErrorHandlingMiddleware به پاسخ HTTP 404 Not Found تبدیل می‌شود.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string name, object key)
        : base($"موجودیت \"{name}\" با کلید ({key}) یافت نشد.")
    {
    }
}