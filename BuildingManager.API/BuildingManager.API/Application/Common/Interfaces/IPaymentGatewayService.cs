using System.Threading.Tasks;

namespace BuildingManager.API.Application.Common.Interfaces;

// نتیجه عملیات پرداخت را مدل می‌کند
public record PaymentResult(bool IsSuccess, string TransactionId, string ErrorMessage = "");

public interface IPaymentGatewayService
{
    Task<PaymentResult> ProcessPaymentAsync(decimal amount, string currency, string description);
}