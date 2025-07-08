using BuildingManager.API.Application.Common.Interfaces;
using System;
using System.Threading.Tasks;

namespace BuildingManager.API.Infrastructure.Services;

public class FakePaymentGatewayService : IPaymentGatewayService
{
    public Task<PaymentResult> ProcessPaymentAsync(decimal amount, string currency, string description)
    {
        // شبیه‌سازی یک عملیات پرداخت موفق
        var isSuccess = true;

        // ایجاد یک کد پیگیری جعلی و منحصر به فرد
        var fakeTransactionId = $"FPG_{Guid.NewGuid().ToString().Substring(0, 12)}";

        var result = new PaymentResult(isSuccess, fakeTransactionId);

        // Task.FromResult برای برگرداندن یک نتیجه از پیش آماده شده در یک متد Async استفاده می‌شود
        return Task.FromResult(result);
    }
}