using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt; // Alias for BCrypt

namespace BuildingManager.API.Application.Features.Authentication.Commands.Register
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly IOtpService _otpService;

        public RegisterUserCommandHandler(IApplicationDbContext context, IOtpService otpService)
        {
            _context = context;
            _otpService = otpService;
        }

        public async Task<RegisterUserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // 1. Check for uniqueness of PhoneNumber and NationalId
            if (await _context.Users.AnyAsync(u => u.PhoneNumber == request.PhoneNumber && !u.IsDeleted, cancellationToken))
            {
                return new RegisterUserResponse { Succeeded = false, Message = "شماره موبایل قبلا ثبت شده است." };
            }

            if (await _context.Users.AnyAsync(u => u.NationalId == request.NationalId && !u.IsDeleted, cancellationToken))
            {
                return new RegisterUserResponse { Succeeded = false, Message = "کد ملی قبلا ثبت شده است." };
            }

            // 2. Create User entity
            var user = new User(
                firstName: request.FirstName,
                lastName: request.LastName,
                nationalId: request.NationalId,
                phoneNumber: request.PhoneNumber,
                passwordHash: BC.HashPassword(request.Password) // Hash the password
            )
            {
                PhoneNumberConfirmed = false, // Must be confirmed via OTP
                IsActive = true, // Active by default, can be changed by admin or other processes
                CreatedAt = System.DateTime.UtcNow
            };

            // 3. Add to DbContext and Save
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            // 4. Generate and send OTP
            // For simplicity, using a constant purpose for phone number confirmation OTPs
            const string otpPurpose = "PhoneNumberConfirmation";
            var otp = await _otpService.GenerateOtpAsync(otpPurpose, user.PhoneNumber);
            await _otpService.SendOtpNotificationAsync(user.PhoneNumber, otp);

            return new RegisterUserResponse
            {
                Succeeded = true,
                Message = "ثبت نام با موفقیت انجام شد. کد تایید به شماره موبایل شما ارسال گردید.",
                UserId = user.PublicId.ToString()
            };
        }
    }
}
