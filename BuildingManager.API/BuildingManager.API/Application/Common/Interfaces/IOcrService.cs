using System.Threading.Tasks;

namespace BuildingManager.API.Application.Common.Interfaces;

/// <summary>
/// اینترفیسی برای سرویس‌های تشخیص کاراکتر نوری (OCR) که برای خواندن پلاک خودرو استفاده می‌شود.
/// </summary>
public interface IOcrService
{
    /// <summary>
    /// شماره پلاک را از روی آدرس URL یک عکس استخراج می‌کند.
    /// </summary>
    /// <param name="imageUrl">آدرس URL عکسی که باید پردازش شود.</param>
    /// <returns>رشته متنی شماره پلاک خوانده شده.</returns>
    Task<string> ReadLicensePlateFromUrlAsync(string imageUrl);
}