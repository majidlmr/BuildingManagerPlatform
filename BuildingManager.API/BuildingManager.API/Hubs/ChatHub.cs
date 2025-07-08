// File: Hubs/ChatHub.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace BuildingManager.API.Hubs;

/// <summary>
/// هاب اصلی SignalR برای مدیریت ارتباطات لحظه‌ای (Real-time) در سیستم چت.
/// این کلاس مسئول دریافت پیام‌ها از یک کلاینت و ارسال آن‌ها به سایر کلاینت‌های مربوطه است.
/// </summary>
[Authorize] // فقط کاربران احراز هویت شده می‌توانند به این هاب متصل شوند
public class ChatHub : Hub
{
    /// <summary>
    /// این متد زمانی فراخوانی می‌شود که یک کاربر جدید به هاب متصل می‌شود.
    /// ما از این متد برای افزودن کاربر به گروه‌های مربوط به خودش استفاده می‌کنیم.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        // شناسه کاربر را از توکن JWT او استخراج می‌کنیم
        var userId = Context.User.FindFirst("sub")?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            // هر کاربر به یک گروه خصوصی با نام شناسه خودش اضافه می‌شود.
            // این کار به ما اجازه می‌دهد تا بتوانیم به یک کاربر خاص پیام مستقیم ارسال کنیم.
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }

        // TODO: در آینده، می‌توانیم کاربر را به صورت خودکار به گروه ساختمانش نیز اضافه کنیم.
        // var buildingId = ...;
        // await Groups.AddToGroupAsync(Context.ConnectionId, $"building-{buildingId}");

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// این متد زمانی فراخوانی می‌شود که اتصال یک کاربر با هاب قطع می‌شود.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // می‌توان در اینجا منطق مربوط به لاگ کردن یا اطلاع‌رسانی قطع اتصال را پیاده‌سازی کرد.
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// این متد توسط کلاینت‌ها برای ارسال پیام فراخوانی می‌شود.
    /// ما در اینجا این متد را خالی رها می‌کنیم، زیرا منطق ارسال پیام را برای امنیت
    /// و پایداری بیشتر، در یک Command جداگانه پیاده‌سازی خواهیم کرد.
    /// </summary>
    public async Task SendMessage(string user, string message)
    {
        // این متد به صورت مستقیم استفاده نخواهد شد.
        // منطق اصلی در SendMessageCommandHandler خواهد بود.
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}