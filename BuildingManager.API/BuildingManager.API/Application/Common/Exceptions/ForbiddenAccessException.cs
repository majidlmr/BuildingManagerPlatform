// File: Application/Common/Exceptions/ForbiddenAccessException.cs
using System;

namespace BuildingManager.API.Application.Common.Exceptions;

/// <summary>
/// خطای سفارشی برای مواردی که کاربر اجازه دسترسی به یک منبع را ندارد.
/// این خطا توسط ErrorHandlingMiddleware به پاسخ HTTP 403 Forbidden تبدیل می‌شود.
/// </summary>
public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException() : base("شما اجازه دسترسی به این منبع را ندارید.") { }

    public ForbiddenAccessException(string message) : base(message) { }
}