using MediatR;
namespace BuildingManager.API.Application.Features.Users.Commands.Login;

public record LoginCommand(string PhoneNumber, string Password) : IRequest<string>;