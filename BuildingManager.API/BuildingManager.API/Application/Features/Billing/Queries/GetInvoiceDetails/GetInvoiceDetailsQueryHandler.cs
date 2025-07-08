// File: Application/Features/Billing/Queries/GetInvoiceDetails/GetInvoiceDetailsQueryHandler.cs
using BuildingManager.API.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Billing.Queries.GetInvoiceDetails;

/// <summary>
/// پردازشگر دستور دریافت جزئیات صورتحساب.
/// این نسخه شامل بررسی کامل و بهینه دسترسی کاربر است.
/// </summary>
public class GetInvoiceDetailsQueryHandler : IRequestHandler<GetInvoiceDetailsQuery, InvoiceDetailsDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuthorizationService _authService;

    public GetInvoiceDetailsQueryHandler(IApplicationDbContext context, IAuthorizationService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<InvoiceDetailsDto> Handle(GetInvoiceDetailsQuery request, CancellationToken cancellationToken)
    {
        // 🚀 مهم‌ترین تغییر: فراخوانی متد با نام صحیح از اینترفیس
        // نام متد از IsInvoiceOwnerAsync به CanAccessInvoiceAsync تغییر کرده است.
        var canAccess = await _authService.CanAccessInvoiceAsync(request.RequestingUserId, request.InvoicePublicId, cancellationToken);
        if (!canAccess)
        {
            throw new Exception("You are not authorized to view this invoice.");
        }

        // پس از تایید دسترسی، اطلاعات صورتحساب را واکشی می‌کنیم.
        var invoice = await _context.Invoices
            .AsNoTracking()
            .Include(i => i.User)
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.PublicId == request.InvoicePublicId, cancellationToken);

        if (invoice == null)
        {
            throw new Exception("Invoice not found.");
        }

        // تبدیل به DTO و بازگرداندن نتیجه (منطق این بخش بدون تغییر است)
        return new InvoiceDetailsDto
        {
            PublicId = invoice.PublicId,
            Description = invoice.Description,
            TotalAmount = invoice.Amount,
            Status = invoice.Status,
            IssueDate = invoice.IssueDate,
            DueDate = invoice.DueDate,
            BilledTo = invoice.User.FullName,
            Items = invoice.Items.Select(item => new InvoiceItemDto(
                item.Description,
                item.Amount,
                item.Type.ToString()
            )).ToList()
        };
    }
}