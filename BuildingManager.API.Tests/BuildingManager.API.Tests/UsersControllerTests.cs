using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BuildingManager.API.Application.Features.Users.Commands.Register;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;

namespace BuildingManager.API.Tests;

public class UsersControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public UsersControllerTests(WebApplicationFactory<Program> factory)
    {
        // این کلاس به صورت خودکار API شما را در حافظه اجرا کرده و یک HttpClient برای ارسال درخواست به آن فراهم می‌کند.
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_ShouldReturnSuccess()
    {
        // Arrange (آماده‌سازی)
        // یک شماره موبایل منحصر به فرد برای هر بار اجرای تست ایجاد می‌کنیم
        var phoneNumber = $"0912{new Random().Next(1000000, 9999999)}";
        var command = new RegisterUserCommand("کاربر تستی", phoneNumber, "Password123!");

        // Act (اجرای عملیات)
        var response = await _client.PostAsJsonAsync("/api/users/register", command);

        // Assert (بررسی نتیجه)
        // بررسی می‌کنیم که آیا درخواست با موفقیت (کد 201) انجام شده است یا خیر
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        // بررسی می‌کنیم که آیا خروجی شامل شناسه کاربر جدید است یا خیر
        var result = await response.Content.ReadFromJsonAsync<object>();
        Assert.NotNull(result);
        Assert.Contains("userId", result.ToString());
    }
}