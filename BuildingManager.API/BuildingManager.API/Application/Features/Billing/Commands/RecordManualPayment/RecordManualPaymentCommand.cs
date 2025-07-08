using MediatR;
using System;

namespace BuildingManager.API.Application.Features.Billing.Commands.RecordManualPayment;

public record RecordManualPaymentCommand(
    Guid InvoicePublicId, // شناسه صورتحسابی که پرداخت برای آن ثبت می‌شود
    decimal Amount,       // مبلغ پرداخت شده
    DateTime PaidAt,      // تاریخ پرداخت
    string? Notes,        // یادداشت مدیر
    int RequestingUserId  // شناسه مدیر برای ثبت در تاریخچه
) : IRequest; // این دستور نتیجه خاصی برنمی‌گرداند