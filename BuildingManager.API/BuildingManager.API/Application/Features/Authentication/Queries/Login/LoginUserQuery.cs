using MediatR;

namespace BuildingManager.API.Application.Features.Authentication.Queries.Login
{
    public class LoginUserQuery : IRequest<LoginUserResponse>
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }

    public class LoginUserResponse
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public string? Token { get; set; }
        public string? UserId { get; set; } // PublicId
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        // Add other relevant user info or roles if needed in the login response
    }
}
