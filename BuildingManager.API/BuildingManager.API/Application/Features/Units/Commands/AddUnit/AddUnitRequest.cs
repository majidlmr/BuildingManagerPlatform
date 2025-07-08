// File: Application/Features/Units/Commands/AddUnit/AddUnitRequest.cs
namespace BuildingManager.API.Application.Features.Units.Commands.AddUnit;

public record AddUnitRequest(
    string UnitNumber,
    string OwnershipStatus,
    int OwnerUserId,
    string? UnitType,
    int? FloorNumber,
    int? Bedrooms,
    decimal? Area
);