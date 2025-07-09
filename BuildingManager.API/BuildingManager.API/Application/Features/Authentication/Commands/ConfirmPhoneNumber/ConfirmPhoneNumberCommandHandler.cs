using BuildingManager.API.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Authentication.Commands.ConfirmPhoneNumber
{
    public class ConfirmPhoneNumberCommandHandler : IRequestHandler<ConfirmPhoneNumberCommand, ConfirmPhoneNumberResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly IOtpService _otpService;
        private const string OtpPurposePhoneNumberConfirmation = "PhoneNumberConfirmation";


        public ConfirmPhoneNumberCommandHandler(IApplicationDbContext context, IOtpService otpService)
        {
            _context = context;
            _otpService = otpService;
        }

        public async Task<ConfirmPhoneNumberResponse> Handle(ConfirmPhoneNumberCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber && !u.IsDeleted, cancellationToken);

            if (user == null)
            {
                return new ConfirmPhoneNumberResponse { Succeeded = false, Message = "کاربری با این شماره موبایل یافت نشد." };
            }

            if (user.PhoneNumberConfirmed)
            {
                return new ConfirmPhoneNumberResponse { Succeeded = true, Message = "شماره موبایل قبلا تایید شده است." };
            }

            var isOtpValid = await _otpService.ValidateOtpAsync(OtpPurposePhoneNumberConfirmation, request.PhoneNumber, request.OtpCode);

            if (!isOtpValid)
            {
                return new ConfirmPhoneNumberResponse { Succeeded = false, Message = "کد تایید نامعتبر یا منقضی شده است." };
            }

            user.PhoneNumberConfirmed = true;
            user.UpdatedAt = System.DateTime.UtcNow;
            // user.IsActive = true; // Or ensure user is active if OTP confirmation implies activation. Usually IsActive is separate.

            await _context.SaveChangesAsync(cancellationToken);

            return new ConfirmPhoneNumberResponse { Succeeded = true, Message = "شماره موبایل با موفقیت تایید شد." };
        }
    }
}
