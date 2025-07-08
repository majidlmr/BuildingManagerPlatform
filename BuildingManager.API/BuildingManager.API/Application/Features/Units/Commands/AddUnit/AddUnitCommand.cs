// File: Application/Features/Units/Commands/AddUnit/AddUnitCommand.cs
using MediatR;
namespace BuildingManager.API.Application.Features.Units.Commands.AddUnit;

public record AddUnitCommand(
    int BuildingId,
    int RequestingUserId, // شناسه کاربری که درخواست را داده (برای چک امنیتی)
    string UnitNumber,
    string OwnershipStatus,
    int OwnerUserId,
    string? UnitType,
    int? FloorNumber,
    int? Bedrooms,
    decimal? Area) : IRequest<int>;