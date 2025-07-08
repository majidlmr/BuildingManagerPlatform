// File: Application/Features/Buildings/Queries/GetBuildingWithUnits/UnitDto.cs
namespace BuildingManager.API.Application.Features.Buildings.Queries.GetBuildingWithUnits;

public record UnitDto(
    Guid PublicId,
    string UnitNumber,
    string? UnitType,
    int? FloorNumber,
    int? Bedrooms,
    decimal? Area
);