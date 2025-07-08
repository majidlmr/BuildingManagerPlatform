using BuildingManager.API.Application.Features.Billing.Commands.CreateCycle;
using BuildingManager.API.Application.Features.Billing.Commands.PayInvoice;
using BuildingManager.API.Application.Features.Billing.Commands.RecordManualPayment;
using BuildingManager.API.Application.Features.Billing.Queries.GetInvoiceDetails;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BuildingManager.API.Controllers;

/// <summary>
/// کنترلری برای مدیریت تمام عملیات‌های مالی و صورتحساب‌ها.
/// </summary>
[ApiController]
[Route("api/billing")]
[Authorize]
public class BillingController : ControllerBase
{
    private readonly IMediator _mediator;

    public BillingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// یک چرخه حسابداری جدید ایجاد کرده و صورتحساب‌ها را به صورت هوشمند صادر می‌کند.
    /// </summary>
    [HttpPost("buildings/{buildingId}/cycles")]
    [Authorize(Roles = "BuildingManager")]
    public async Task<IActionResult> CreateBillingCycle(int buildingId, [FromBody] CreateBillingCycleRequest request)
    {
        var requestingUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var command = new CreateBillingCycleCommand(
            buildingId,
            requestingUserId,
            request.Name,
            request.StartDate,
            request.EndDate,
            request.DefaultChargePerUnit
        );

        var cycleId = await _mediator.Send(command);
        return StatusCode(201, new { BillingCycleId = cycleId });
    }

    /// <summary>
    /// یک صورتحساب را از طریق درگاه پرداخت (تستی) پرداخت می‌کند.
    /// </summary>
    [HttpPost("invoices/{invoiceId}/pay")]
    public async Task<IActionResult> PayInvoice(Guid invoiceId)
    {
        var payerUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new PayInvoiceCommand(invoiceId, payerUserId);

        var transactionId = await _mediator.Send(command);

        return Ok(new { TransactionId = transactionId });
    }

    /// <summary>
    /// یک پرداخت دستی را برای یک صورتحساب ثبت می‌کند. (فقط برای مدیر ساختمان)
    /// </summary>
    [HttpPost("invoices/{invoiceId}/manual-payment")]
    [Authorize(Roles = "BuildingManager")]
    public async Task<IActionResult> RecordManualPayment(Guid invoiceId, [FromBody] RecordManualPaymentRequest request)
    {
        var requestingUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new RecordManualPaymentCommand(
            invoiceId,
            request.Amount,
            request.PaidAt,
            request.Notes,
            requestingUserId
        );

        await _mediator.Send(command);

        return Ok(new { message = "پرداخت دستی با موفقیت ثبت شد." });
    }

    /// <summary>
    /// جزئیات کامل یک صورتحساب را به همراه ردیف‌های آن برمی‌گرداند.
    /// </summary>
    [HttpGet("invoices/{invoiceId}")]
    public async Task<IActionResult> GetInvoiceDetails(Guid invoiceId)
    {
        var requestingUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var query = new GetInvoiceDetailsQuery(invoiceId, requestingUserId);

        var invoiceDetails = await _mediator.Send(query);

        return Ok(invoiceDetails);
    }
}