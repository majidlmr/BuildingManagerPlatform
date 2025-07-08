using MediatR;
using System;

namespace BuildingManager.API.Application.Features.Billing.Queries.GetInvoiceDetails;

/// <summary>
/// دستوری برای دریافت جزئیات کامل یک صورتحساب با استفاده از شناسه عمومی آن.
/// </summary>
public record GetInvoiceDetailsQuery(
    Guid InvoicePublicId,
    int RequestingUserId
) : IRequest<InvoiceDetailsDto>;