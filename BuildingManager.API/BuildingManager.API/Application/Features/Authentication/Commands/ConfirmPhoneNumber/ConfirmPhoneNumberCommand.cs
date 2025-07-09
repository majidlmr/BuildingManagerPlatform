using MediatR;

namespace BuildingManager.API.Application.Features.Authentication.Commands.ConfirmPhoneNumber
{
    public class ConfirmPhoneNumberCommand : IRequest<ConfirmPhoneNumberResponse>
    {
        public string PhoneNumber { get; set; } // Or UserId, depending on how you want to identify the user
        public string OtpCode { get; set; }
    }

    public class ConfirmPhoneNumberResponse
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
    }
}
