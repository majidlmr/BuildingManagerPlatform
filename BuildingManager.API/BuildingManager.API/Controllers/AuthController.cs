using BuildingManager.API.Application.Features.Authentication.Commands.Register;
using BuildingManager.API.Application.Features.Authentication.Commands.ConfirmPhoneNumber;
using BuildingManager.API.Application.Features.Authentication.Queries.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace BuildingManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ISender _mediator;

        public AuthController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        [AllowAnonymous] // Ensure this endpoint is accessible without authentication
        public async Task<IActionResult> Register(RegisterUserCommand command)
        {
            var response = await _mediator.Send(command);
            if (!response.Succeeded)
            {
                return BadRequest(new { response.Message });
            }
            return Ok(response);
        }

        [HttpPost("confirm-phonenumber")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmPhoneNumber(ConfirmPhoneNumberCommand command)
        {
            var response = await _mediator.Send(command);
            if (!response.Succeeded)
            {
                return BadRequest(new { response.Message });
            }
            return Ok(response);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginUserQuery query)
        {
            var response = await _mediator.Send(query);
            if (!response.Succeeded)
            {
                // It's generally better not to reveal whether the user exists or password was wrong.
                // So, a generic message for auth failure.
                return Unauthorized(new { Message = response.Message }); // Use Unauthorized for login failures
            }
            return Ok(response);
        }
    }
}
