using MediatR;
using System;

namespace BuildingManager.API.Application.Features.Billing.Commands.PayInvoice;

public record PayInvoiceCommand(
    Guid InvoicePublicId, // شناسه عمومی صورتحسابی که باید پرداخت شود
    int PayerUserId       // شناسه کاربری که پرداخت را انجام می‌دهد
) : IRequest<string>; // کد پیگیری تراکنش را برمی‌گرداند