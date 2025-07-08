using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Billing.Commands.RecordManualPayment;

public class RecordManualPaymentCommandHandler : IRequestHandler<RecordManualPaymentCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public RecordManualPaymentCommandHandler(IApplicationDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RecordManualPaymentCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _context.Invoices
            .FirstOrDefaultAsync(i => i.PublicId == request.InvoicePublicId, cancellationToken);

        if (invoice == null) throw new Exception("Invoice not found.");

        // اگر مبلغ دستی وارد شده با مبلغ فاکتور برابر بود، آن را پرداخت شده تلقی می‌کنیم
        if (invoice.Amount == request.Amount)
        {
            invoice.Status = "Paid";
        }
        else
        {
            // در آینده می‌توان منطق پرداخت قسمتی از وجه را نیز پیاده‌سازی کرد
            invoice.Status = "PartiallyPaid";
        }

        var transaction = new Transaction
        {
            InvoiceId = invoice.Id,
            PayerUserId = request.RequestingUserId, // ثبت کننده مدیر است
            Amount = request.Amount,
            PaymentGateway = "Manual", // مشخص کردن نوع پرداخت
            GatewayRefId = $"MANUAL_{Guid.NewGuid().ToString().Substring(0, 8)}", // یک شناسه یکتا برای پیگیری
            Status = "Succeeded",
            PaidAt = request.PaidAt.ToUniversalTime()
        };

        await _context.Transactions.AddAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}