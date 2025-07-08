// File: Controllers/ConversationsController.cs
using BuildingManager.API.Application.Features.Chat.Commands.DeleteMessage; // ✅ 1. افزودن using برای دستور حذف
using BuildingManager.API.Application.Features.Chat.Commands.SendMessage;
using BuildingManager.API.Application.Features.Chat.Commands.StartConversation;
using BuildingManager.API.Application.Features.Chat.Queries.GetConversationMessages;
using BuildingManager.API.Application.Features.Chat.Queries.GetMyConversations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BuildingManager.API.Controllers;

[ApiController]
[Route("api/conversations")]
[Authorize]
public class ConversationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ConversationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ... (متدهای GetMyConversations, GetConversationMessages, StartConversation بدون تغییر باقی می‌مانند) ...

    /// <summary>
    /// لیست تمام گفتگوهای کاربر لاگین شده را برمی‌گرداند.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<ConversationDto>>> GetMyConversations()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var query = new GetMyConversationsQuery(userId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// تاریخچه پیام‌های یک گفتگوی مشخص را برمی‌گرداند.
    /// </summary>
    [HttpGet("{conversationId:guid}/messages")]
    public async Task<ActionResult<List<MessageDto>>> GetConversationMessages(Guid conversationId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var query = new GetConversationMessagesQuery(conversationId, userId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// یک گفتگوی دو نفره جدید با یک کاربر دیگر شروع می‌کند.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> StartConversation([FromBody] StartConversationRequest request)
    {
        var initiatorUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new StartConversationCommand(initiatorUserId, request.UserId);
        var conversationPublicId = await _mediator.Send(command);
        return Ok(new { conversationId = conversationPublicId });
    }

    /// <summary>
    /// یک پیام جدید در یک گفتگوی مشخص ارسال می‌کند.
    /// </summary>
    [HttpPost("{conversationId:guid}/messages")]
    public async Task<IActionResult> SendMessage(Guid conversationId, [FromBody] SendMessageRequest request)
    {
        var senderUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new SendMessageCommand(conversationId, senderUserId, request.Content, request.IsAnonymous);
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// 🚀 اندپوینت جدید: یک پیام را به صورت منطقی حذف می‌کند.
    /// </summary>
    /// <param name="conversationId">شناسه گفتگو (برای ساختار مسیر).</param>
    /// <param name="messageId">شناسه عمومی پیامی که باید حذف شود.</param>
    [HttpDelete("{conversationId:guid}/messages/{messageId:guid}")]
    public async Task<IActionResult> DeleteMessage(Guid conversationId, Guid messageId)
    {
        var requestingUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new DeleteMessageCommand(messageId, requestingUserId);
        await _mediator.Send(command);
        return NoContent();
    }
}

// مدل ورودی برای شروع یک گفتگوی جدید
public record StartConversationRequest(int UserId);