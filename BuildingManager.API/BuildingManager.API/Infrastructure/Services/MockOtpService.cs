using BuildingManager.API.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BuildingManager.API.Infrastructure.Services
{
    public class MockOtpService : IOtpService
    {
        private readonly ILogger<MockOtpService> _logger;
        // In a real scenario, use a distributed cache like Redis or a database table for OTPs
        private static readonly Dictionary<string, (string Otp, DateTime Expiry)> OtpStore = new();
        private const string OtpPurposePhoneNumberConfirmation = "PhoneNumberConfirmation";

        public MockOtpService(ILogger<MockOtpService> logger)
        {
            _logger = logger;
        }

        public Task<string> GenerateOtpAsync(string purpose, string identifier)
        {
            var otp = new Random().Next(100000, 999999).ToString("D6");
            var key = $"{purpose}:{identifier}";
            OtpStore[key] = (otp, DateTime.UtcNow.AddMinutes(5)); // OTP valid for 5 minutes
            _logger.LogInformation("Generated OTP {Otp} for purpose {Purpose} and identifier {Identifier}", otp, purpose, identifier);
            return Task.FromResult(otp);
        }

        public Task<bool> ValidateOtpAsync(string purpose, string identifier, string otp)
        {
            var key = $"{purpose}:{identifier}";
            if (OtpStore.TryGetValue(key, out var storedOtpInfo))
            {
                if (storedOtpInfo.Otp == otp && storedOtpInfo.Expiry >= DateTime.UtcNow)
                {
                    OtpStore.Remove(key); // OTP used, remove it
                    _logger.LogInformation("OTP {Otp} validated successfully for purpose {Purpose} and identifier {Identifier}", otp, purpose, identifier);
                    return Task.FromResult(true);
                }
                if (storedOtpInfo.Expiry < DateTime.UtcNow)
                {
                    OtpStore.Remove(key); // OTP expired, remove it
                     _logger.LogWarning("OTP {Otp} validation failed for purpose {Purpose} and identifier {Identifier}. Reason: Expired.", otp, purpose, identifier);
                }
                else
                {
                    _logger.LogWarning("OTP {Otp} validation failed for purpose {Purpose} and identifier {Identifier}. Reason: Mismatch.", otp, purpose, identifier);
                }
            }
            else
            {
                _logger.LogWarning("OTP {Otp} validation failed for purpose {Purpose} and identifier {Identifier}. Reason: Not Found.", otp, purpose, identifier);
            }
            return Task.FromResult(false);
        }

        public Task SendOtpNotificationAsync(string phoneNumber, string otp)
        {
            // In a real application, this would use an SMS gateway service.
            // For now, we just log it.
            _logger.LogInformation("Simulating sending OTP to {PhoneNumber}: Your OTP is {Otp}", phoneNumber, otp);
            // In a test environment, this might be written to a file or a specific mock service.
            Console.WriteLine($"DEMO OTP for {phoneNumber}: {otp}");
            return Task.CompletedTask;
        }
    }
}
