using BuildingManager.API.Application.Common.Interfaces;
using System.Threading.Tasks;

namespace BuildingManager.API.Infrastructure.Services;

/// <summary>
/// یک پیاده‌سازی تستی و شبیه‌سازی شده از سرویس OCR برای خواندن پلاک.
/// در نسخه واقعی، این کلاس با کدی که به یک سرویس OCR واقعی متصل می‌شود، جایگزین خواهد شد.
/// </summary>
public class FakeOcrService : IOcrService
{
    public Task<string> ReadLicensePlateFromUrlAsync(string imageUrl)
    {
        // برای اهداف تست، همیشه یک شماره پلاک ثابت را برمی‌گردانیم.
        // این مقدار می‌تواند در آینده از یک لیست تصادفی انتخاب شود.
        string fakePlate = "12-345B78";

        return Task.FromResult(fakePlate);
    }
}