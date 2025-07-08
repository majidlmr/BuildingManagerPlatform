using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Events;
using BuildingManager.API.Domain.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Billing.Commands.PayInvoice;

/// <summary>
/// پردازشگر دستور پرداخت یک صورتحساب.
/// این کلاس مسئولیت‌های زیر را بر عهده دارد:
/// 1. بررسی وضعیت صورتحساب.
/// 2. فراخوانی سرویس درگاه پرداخت.
/// 3. ثبت تراکنش موفق در دیتابیس.
/// 4. انتشار رویداد 'InvoicePaidEvent' برای اطلاع‌رسانی به سایر ماژول‌ها.
/// </summary>
public class PayInvoiceCommandHandler : IRequestHandler<PayInvoiceCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly IPaymentGatewayService _paymentGateway;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublisher _publisher;

    public PayInvoiceCommandHandler(
        IApplicationDbContext context,
        IPaymentGatewayService paymentGateway,
        IUnitOfWork unitOfWork,
        IPublisher publisher)
    {
        _context = context;
        _paymentGateway = paymentGateway;
        _unitOfWork = unitOfWork;
        _publisher = publisher;
    }

    public async Task<string> Handle(PayInvoiceCommand request, CancellationToken cancellationToken)
    {
        // گام ۱: پیدا کردن صورتحساب و بررسی وضعیت آن
        var invoice = await _context.Invoices
            .FirstOrDefaultAsync(i => i.PublicId == request.InvoicePublicId, cancellationToken);

        if (invoice == null)
        {
            throw new NotFoundException("صورتحساب یافت نشد.");
        }
        if (invoice.Status == "Paid")
        {
            throw new ValidationException("این صورتحساب قبلاً پرداخت شده است.");
        }

        // گام ۲: فراخوانی سرویس درگاه پرداخت برای انجام عملیات پرداخت
        var paymentResult = await _paymentGateway.ProcessPaymentAsync(invoice.Amount, "IRR", invoice.Description);

        if (!paymentResult.IsSuccess)
        {
            // در صورت بروز خطا در درگاه پرداخت، یک خطای عمومی ایجاد می‌کنیم
            // زیرا این خطا معمولاً یک مشکل زیرساختی یا خارجی است.
            throw new Exception($"پرداخت با درگاه ناموفق بود: {paymentResult.ErrorMessage}");
        }

        // گام ۳: تغییر وضعیت صورتحساب به "پرداخت شده"
        invoice.Status = "Paid";

        // گام ۴: ایجاد و ثبت رکورد تراکنش موفق
        var transaction = new Transaction
        {
            InvoiceId = invoice.Id,
            PayerUserId = request.PayerUserId,
            Amount = invoice.Amount,
            PaymentGateway = "FakeGateway", // در آینده نام درگاه واقعی اینجا قرار میگیرد
            GatewayRefId = paymentResult.TransactionId,
            Status = "Succeeded",
            PaidAt = DateTime.UtcNow
        };

        await _context.Transactions.AddAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // گام ۵ (مهم): انتشار رویداد "صورتحساب پرداخت شد"
        // پس از ذخیره موفقیت‌آمیز، به تمام سیستم اعلام می‌کنیم که یک پرداخت انجام شده است.
        var invoicePaidEvent = new InvoicePaidEvent(invoice.Id, request.PayerUserId, invoice.Amount);
        await _publisher.Publish(invoicePaidEvent, cancellationToken);

        // گام ۶: بازگرداندن کد پیگیری تراکنش به کلاینت
        return transaction.GatewayRefId;
    }
}