using BuildingManager.API.Application.Features.Rules.Commands.AcknowledgeRule;
using BuildingManager.API.Application.Features.Rules.Commands.CreateRule;
using BuildingManager.API.Application.Features.Rules.Queries.GetRules;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BuildingManager.API.Controllers;

[ApiController]
[Route("api/buildings/{buildingId}/rules")]
[Authorize]
public class RulesController : ControllerBase
{
    private readonly IMediator _mediator;

    public RulesController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [Authorize(Roles = "BuildingManager")]
    public async Task<IActionResult> CreateRule(int buildingId, [FromBody] CreateRuleRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new CreateRuleCommand(buildingId, request.Title, request.Content, userId);
        var ruleId = await _mediator.Send(command);
        return StatusCode(201, new { RuleId = ruleId });
    }

    [HttpGet]
    public async Task<IActionResult> GetRules(int buildingId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var query = new GetRulesQuery(buildingId, userId);
        var rules = await _mediator.Send(query);
        return Ok(rules);
    }
    /// <summary>
    /// مطالعه و پذیرش یک قانون توسط کاربر را ثبت می‌کند.
    /// </summary>
    /// <param name="buildingId">شناسه ساختمان (برای مسیر).</param>
    /// <param name="ruleId">شناسه قانونی که تایید می‌شود.</param>
    [HttpPost("{ruleId}/acknowledge")]
    public async Task<IActionResult> AcknowledgeRule(int buildingId, int ruleId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // buildingId در اینجا برای سازگاری با ساختار مسیر استفاده شده است
        var command = new AcknowledgeRuleCommand(ruleId, userId);

        await _mediator.Send(command);

        return Ok(new { message = "قانون با موفقیت تایید شد." });
    }
}

public record CreateRuleRequest(string Title, string Content);