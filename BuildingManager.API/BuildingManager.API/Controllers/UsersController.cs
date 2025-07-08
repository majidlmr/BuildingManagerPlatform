using BuildingManager.API.Application.Features.Users.Commands.ConfirmPhoneNumber;
using BuildingManager.API.Application.Features.Users.Commands.Login;
using BuildingManager.API.Application.Features.Users.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BuildingManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserCommand command)
    {
        var userId = await _mediator.Send(command);
        return StatusCode(201, new { UserId = userId });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        var token = await _mediator.Send(command);
        return Ok(new { Token = token });
    }

    [HttpPost("confirm-phonenumber")]
    public async Task<IActionResult> ConfirmPhoneNumber(ConfirmPhoneNumberCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { message = "شماره موبایل شما با موفقیت تایید شد. اکنون می‌توانید وارد شوید." });
    }
}