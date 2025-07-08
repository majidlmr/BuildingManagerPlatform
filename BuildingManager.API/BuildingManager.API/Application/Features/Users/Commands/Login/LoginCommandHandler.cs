using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;  
using System.Linq;                 
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Users.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
{
    private readonly IApplicationDbContext _context; // ✅ برای بارگذاری روابط، از DbContext استفاده می‌کنیم
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(IApplicationDbContext context, IJwtTokenGenerator jwtTokenGenerator)
    {
        _context = context;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // ✅ تغییر اصلی: هنگام واکشی کاربر، نقش‌های او را نیز با Include بارگذاری می‌کنیم
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber, cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new ValidationException("شماره موبایل یا رمز عبور نامعتبر است.");
        }

        if (!user.PhoneNumberConfirmed)
        {
            throw new ValidationException("حساب کاربری شما فعال نشده است. لطفاً ابتدا شماره موبایل خود را تایید کنید.");
        }

        var token = _jwtTokenGenerator.GenerateToken(user);
        return token;
    }
}