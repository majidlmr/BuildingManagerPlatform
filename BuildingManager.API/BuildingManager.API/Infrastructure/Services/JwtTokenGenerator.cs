using BuildingManager.API.Application.Common.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BuildingManager.API.Infrastructure.Services
{
    // This class might be better placed in a Domain/ValueObjects or a dedicated ConfigurationModels folder.
    public class JwtSettings
    {
        public const string SectionName = "JwtSettings";
        public string Secret { get; init; } = null!;
        public string Issuer { get; init; } = null!;
        public string Audience { get; init; } = null!;
        public int ExpiryHours { get; init; } = 8; // Default expiry to 8 hours
    }

    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtSettings _jwtSettings;

        public JwtTokenGenerator(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
            if (string.IsNullOrEmpty(_jwtSettings.Secret) || _jwtSettings.Secret.Length < 32) // Example minimum length
            {
                throw new ArgumentException("JWT Secret must be configured and be of sufficient length.", nameof(jwtSettings));
            }
        }

        public string GenerateToken(int userId, Guid publicId, string firstName, string lastName, string phoneNumber, IReadOnlyList<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userId.ToString()), // Subject (usually user's unique ID)
                new(JwtRegisteredClaimNames.NameId, publicId.ToString()), // Using PublicId as NameIdentifier
                new("uid", userId.ToString()), // Custom claim for integer UserId if needed elsewhere
                new(JwtRegisteredClaimNames.GivenName, firstName),
                new(JwtRegisteredClaimNames.FamilyName, lastName),
                new("phone_number", phoneNumber), // Using "phone_number" claim for phone
                // new(JwtRegisteredClaimNames.Email, email ?? string.Empty), // If email is passed
            };

            if (roles != null)
            {
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role)); // Using ClaimTypes.Role for standard role claim
                }
            }

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature); // More specific algorithm

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpiryHours),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = creds
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}