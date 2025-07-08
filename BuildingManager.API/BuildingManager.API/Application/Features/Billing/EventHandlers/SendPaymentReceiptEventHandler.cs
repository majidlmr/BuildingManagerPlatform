using BuildingManager.API.Domain.Events;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Billing.EventHandlers;

/// <summary>
/// این Handler به رویداد InvoicePaidEvent گوش می‌دهد و وظیفه شبیه‌سازی
/// ارسال رسید پرداخت برای کاربر را بر عهده دارد.
/// </summary>
public class SendPaymentReceiptEventHandler : INotificationHandler<InvoicePaidEvent>
{
    public Task Handle(InvoicePaidEvent notification, CancellationToken cancellationToken)
    {
        // در یک پروژه واقعی، اینجا کد اتصال به سرویس ایمیل یا پیامک قرار می‌گیرد.
        Console.WriteLine($"[EMAIL-SIMULATION] Sending payment receipt for InvoiceId: {notification.InvoiceId} to UserId: {notification.UserId}. Amount: {notification.AmountPaid}");
        return Task.CompletedTask;
    }
}