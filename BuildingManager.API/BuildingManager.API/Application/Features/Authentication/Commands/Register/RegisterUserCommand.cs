using MediatR;

namespace BuildingManager.API.Application.Features.Authentication.Commands.Register
{
    public class RegisterUserCommand : IRequest<RegisterUserResponse> // Assuming a response DTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NationalId { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    // Placeholder for the response, can be expanded later
    public class RegisterUserResponse
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; } // PublicId of the user
    }
}
