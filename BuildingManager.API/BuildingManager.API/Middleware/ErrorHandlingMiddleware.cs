// File: Middleware/ErrorHandlingMiddleware.cs
using BuildingManager.API.Application.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace BuildingManager.API.Middleware;

/// <summary>
/// یک میان‌افزار متمرکز برای مدیریت تمام خطاها در برنامه.
/// این کلاس خطاها را گرفته و آن‌ها را به یک پاسخ JSON استاندارد و قابل فهم برای کلاینت تبدیل می‌کند.
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // به صورت پیش‌فرض، کد وضعیت 500 (خطای داخلی سرور) در نظر گرفته می‌شود.
        var code = HttpStatusCode.InternalServerError;
        object result;

        switch (exception)
        {
            // اگر خطا از نوع خطاهای اعتبارسنجی FluentValidation بود
            case ValidationException validationException:
                code = HttpStatusCode.BadRequest; // 400 Bad Request
                var validationErrors = validationException.Errors
                    .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                    .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
                result = new { title = "خطای اعتبارسنجی", status = (int)code, errors = validationErrors };
                break;

            // 🚀 اگر خطا از نوع "عدم دسترسی" بود که خودمان تعریف کردیم
            case ForbiddenAccessException forbiddenException:
                code = HttpStatusCode.Forbidden; // 403 Forbidden
                result = new { title = "عدم دسترسی", status = (int)code, detail = forbiddenException.Message };
                break;

            // 🚀 اگر خطا از نوع "یافت نشد" بود که خودمان تعریف کردیم
            case NotFoundException notFoundException:
                code = HttpStatusCode.NotFound; // 404 Not Found
                result = new { title = "یافت نشد", status = (int)code, detail = notFoundException.Message };
                break;

            // برای سایر خطاهای پیش‌بینی نشده
            default:
                result = new { title = "خطای پیش‌بینی نشده سرور", status = (int)code, detail = exception.Message };
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        // استفاده از گزینه‌های سریالایزر برای خوانایی بهتر خروجی JSON
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };

        return context.Response.WriteAsync(JsonSerializer.Serialize(result, options));
    }
}