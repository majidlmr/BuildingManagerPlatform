using BuildingManager.API.Domain.Events;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Users.EventHandlers;

/// <summary>
/// این Handler به رویداد UserRegisteredEvent گوش می‌دهد
/// و وظیفه ارسال ایمیل خوشامدگویی به کاربر جدید را بر عهده دارد.
/// </summary>
public class SendWelcomeEmailEventHandler : INotificationHandler<UserRegisteredEvent>
{
    public Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        // در یک پروژه واقعی، اینجا کد مربوط به اتصال به سرویس ایمیل و ارسال آن قرار می‌گیرد.
        // ما در اینجا صرفاً آن را در کنسول شبیه‌سازی می‌کنیم.
        Console.WriteLine($"[EMAIL-SIMULATION] Sending welcome email to: {notification.User.FullName} ({notification.User.PhoneNumber})...");

        // این عملیات می‌تواند به صورت غیرهمزمان و در پس‌زمینه انجام شود.
        return Task.CompletedTask;
    }
}