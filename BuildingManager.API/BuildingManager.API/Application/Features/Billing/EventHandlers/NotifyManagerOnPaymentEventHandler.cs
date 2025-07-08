using BuildingManager.API.Domain.Events;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Billing.EventHandlers;

/// <summary>
/// این Handler به رویداد InvoicePaidEvent گوش می‌دهد و وظیفه شبیه‌سازی
/// ارسال نوتیفیکیشن به مدیر ساختمان را بر عهده دارد.
/// </summary>
public class NotifyManagerOnPaymentEventHandler : INotificationHandler<InvoicePaidEvent>
{
    public Task Handle(InvoicePaidEvent notification, CancellationToken cancellationToken)
    {
        // در یک پروژه واقعی، اینجا کد ارسال نوتیفیکیشن از طریق SignalR یا سرویس‌های دیگر قرار می‌گیرد.
        Console.WriteLine($"[NOTIFICATION-SIMULATION] Notifying building managers about payment for InvoiceId: {notification.InvoiceId}.");
        return Task.CompletedTask;
    }
}