namespace BuildingManager.API.Application.Features.Vehicles.Commands.CreateVehicle;

/// <summary>
/// مدلی که اطلاعات خودرو را از کلاینت دریافت می‌کند.
/// </summary>
public record CreateVehicleRequest(
    string LicensePlate,
    string? Model,
    string? Color,
    string? Description
);