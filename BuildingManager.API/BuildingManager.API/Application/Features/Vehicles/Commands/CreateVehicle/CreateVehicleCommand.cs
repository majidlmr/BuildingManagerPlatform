using MediatR;

namespace BuildingManager.API.Application.Features.Vehicles.Commands.CreateVehicle;

/// <summary>
/// دستوری برای ثبت یک وسیله نقلیه جدید برای کاربر.
/// </summary>
public record CreateVehicleCommand(
    int UserId,
    string LicensePlate,
    string? Model,
    string? Color,
    string? Description
) : IRequest<int>;