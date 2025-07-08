using MediatR;
namespace BuildingManager.API.Application.Features.Users.Commands.Register;

public record RegisterUserCommand(string FullName, string PhoneNumber, string Password) : IRequest<int>;