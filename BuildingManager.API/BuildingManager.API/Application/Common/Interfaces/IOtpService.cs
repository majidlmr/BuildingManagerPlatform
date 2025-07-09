using System.Threading.Tasks;

namespace BuildingManager.API.Application.Common.Interfaces
{
    public interface IOtpService
    {
        Task<string> GenerateOtpAsync(string purpose, string identifier); // Generates and stores OTP
        Task<bool> ValidateOtpAsync(string purpose, string identifier, string otp); // Validates OTP
        Task SendOtpNotificationAsync(string phoneNumber, string otp); // Sends OTP via SMS/Email (mocked for now)
    }
}
