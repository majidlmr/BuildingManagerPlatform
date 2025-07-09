using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt; // Alias for BCrypt

namespace BuildingManager.API.Application.Features.Authentication.Queries.Login
{
    public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, LoginUserResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public LoginUserQueryHandler(IApplicationDbContext context, IJwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<LoginUserResponse> Handle(LoginUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber && !u.IsDeleted, cancellationToken);

            if (user == null)
            {
                return new LoginUserResponse { Succeeded = false, Message = "شماره موبایل یا رمز عبور نامعتبر است." };
            }

            if (!user.IsActive)
            {
                return new LoginUserResponse { Succeeded = false, Message = "حساب کاربری شما غیرفعال شده است." };
            }

            if (!user.PhoneNumberConfirmed)
            {
                // Optionally, resend OTP here or prompt user to confirm phone first
                return new LoginUserResponse { Succeeded = false, Message = "شماره موبایل شما هنوز تایید نشده است. لطفا ابتدا شماره موبایل خود را تایید کنید." };
            }

            if (!BC.Verify(request.Password, user.PasswordHash))
            {
                return new LoginUserResponse { Succeeded = false, Message = "شماره موبایل یا رمز عبور نامعتبر است." };
            }

            // Update LastLoginAt
            user.LastLoginAt = System.DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken); // Save changes for LastLoginAt

            // Generate JWT Token
            // You might want to fetch user roles here to include in the token claims if needed.
            // For now, just basic user info.
            var token = _jwtTokenGenerator.GenerateToken(user.Id, user.PublicId, user.FirstName, user.LastName, user.PhoneNumber, new List<string>()); // Pass empty list of roles for now

            return new LoginUserResponse
            {
                Succeeded = true,
                Message = "ورود با موفقیت انجام شد.",
                Token = token,
                UserId = user.PublicId.ToString(),
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }
    }
}
