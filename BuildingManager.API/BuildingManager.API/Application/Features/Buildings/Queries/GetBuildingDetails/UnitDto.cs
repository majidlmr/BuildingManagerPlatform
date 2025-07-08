// File: Application/Features/Buildings/Queries/GetBuildingDetails/UnitDto.cs
namespace BuildingManager.API.Application.Features.Buildings.Queries.GetBuildingDetails;

public record UnitDto(
    Guid PublicId,
    string UnitNumber,
    string? UnitType,
    int? FloorNumber,
    int? Bedrooms,
    decimal? Area,
    string OwnershipStatus
);