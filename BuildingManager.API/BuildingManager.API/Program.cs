// File: Program.cs
using BuildingManager.API.Application.Common.Behaviors;
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Interfaces;
using BuildingManager.API.Hubs;
using BuildingManager.API.Infrastructure.Persistence.DbContexts;
using BuildingManager.API.Infrastructure.Persistence.Repositories;
using BuildingManager.API.Infrastructure.Services;
using BuildingManager.API.Middleware;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using BuildingManager.API.Infrastructure.Services.ChargeCalculation;

var builder = WebApplication.CreateBuilder(args);

// =================================================================
// بخش ۱: ثبت سرویس‌ها در کانتینر (Dependency Injection)
// =================================================================

builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// پیکربندی Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token. Example: \"Bearer {token}\"",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            new string[]{}
        }
    });
    options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
});

// ثبت تنظیمات و سرویس JWT
var jwtSettings = new JwtSettings();
builder.Configuration.Bind(JwtSettings.SectionName, jwtSettings);
builder.Services.AddSingleton(Options.Create(jwtSettings));
builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

// پیکربندی سیستم احراز هویت
builder.Services
    .AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
    });

// ثبت MediatR و پایپ‌لاین اعتبارسنجی
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(IApplicationDbContext).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});

// ثبت اعتبارسنج‌های FluentValidation
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// ✅ (اصلاح شده) ثبت DbContext با سینتکس صحیح
builder.Services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlServerOptionsAction: sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
    }));

// ثبت Repository ها و UnitOfWork
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBuildingRepository, BuildingRepository>();
builder.Services.AddScoped<IUnitRepository, UnitRepository>();
builder.Services.AddScoped<IBillingCycleRepository, BillingCycleRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ثبت استراتژی‌های محاسبه شارژ
builder.Services.AddScoped<IChargeCalculationStrategy, EqualChargeStrategy>();
builder.Services.AddScoped<IChargeCalculationStrategy, AreaBasedChargeStrategy>();

// ثبت سرویس‌های جانبی
builder.Services.AddScoped<IPaymentGatewayService, FakePaymentGatewayService>();
builder.Services.AddScoped<IOcrService, FakeOcrService>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
builder.Services.AddSingleton<IOtpService, MockOtpService>(); // Added MockOtpService

// =================================================================
// بخش ۲: ساخت اپلیکیشن
// =================================================================
var app = builder.Build();

// Seed Data in Development Environment
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            // Ensure database is created/migrated before seeding
            // context.Database.Migrate(); // Or context.Database.EnsureCreated(); for development

            var logger = services.GetRequiredService<ILogger<BuildingManager.API.Infrastructure.Persistence.ApplicationDbContextSeed>>();
            await BuildingManager.API.Infrastructure.Persistence.ApplicationDbContextSeed.SeedSampleDataAsync(context, logger);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }
}

// =================================================================
// بخش ۳: پیکربندی خط لوله درخواست (Request Pipeline)
// =================================================================

app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ChatHub>("/chathub");

app.Run();

// =================================================================
// بخش ۴: تعریف کلاس Partial برای دسترسی در تست‌های یکپارچه
// =================================================================
public partial class Program { }