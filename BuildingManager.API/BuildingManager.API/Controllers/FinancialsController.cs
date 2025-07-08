using BuildingManager.API.Application.Features.Financials.Commands.CreateExpense;
using BuildingManager.API.Application.Features.Financials.Commands.CreateRevenue;
using BuildingManager.API.Application.Features.Financials.Queries.GetFinancialSummary;
using BuildingManager.API.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BuildingManager.API.Domain.Constants;

namespace BuildingManager.API.Controllers;

[ApiController]
[Route("api/financials")]
[Authorize(Roles = UserRoles.BuildingManager)]
public class FinancialsController : ControllerBase
{
    private readonly IMediator _mediator;

    public FinancialsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// یک هزینه جدید برای ساختمان ثبت می‌کند.
    /// </summary>
    [HttpPost("buildings/{buildingId}/expenses")]
    public async Task<IActionResult> RecordExpense(int buildingId, [FromBody] CreateExpenseRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var command = new CreateExpenseCommand(
            buildingId,
            request.Title,
            request.Description,
            request.Amount,
            request.ExpenseDate,
            request.Category,
            request.AllocationMethod,
            userId
        );

        var expenseId = await _mediator.Send(command);

        return StatusCode(201, new { ExpenseId = expenseId });
    }

    /// <summary>
    /// یک درآمد جدید برای ساختمان ثبت می‌کند.
    /// </summary>
    [HttpPost("buildings/{buildingId}/revenues")]
    public async Task<IActionResult> RecordRevenue(int buildingId, [FromBody] CreateRevenueRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var command = new CreateRevenueCommand(
            buildingId,
            request.Title,
            request.Description,
            request.Amount,
            request.RevenueDate,
            request.Category,
            userId
        );

        var revenueId = await _mediator.Send(command);

        return StatusCode(201, new { RevenueId = revenueId });
    }

    /// <summary>
    /// گزارش بیلان مالی ساختمان را در یک بازه زمانی مشخص برمی‌گرداند.
    /// </summary>
    [HttpGet("buildings/{buildingId}/summary")]
    public async Task<IActionResult> GetFinancialSummary(int buildingId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var query = new GetFinancialSummaryQuery(buildingId, startDate, endDate, userId);
        var summary = await _mediator.Send(query);
        return Ok(summary);
    }
}