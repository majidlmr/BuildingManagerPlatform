using System;

namespace BuildingManager.API.Application.Features.Billing.Commands.RecordManualPayment;

// اطلاعاتی که مدیر در فرم وارد می‌کند
public record RecordManualPaymentRequest(
    decimal Amount,
    DateTime PaidAt,
    string? Notes
);