// File: Application/Features/Buildings/Commands/Create/CreateBuildingRequest.cs
namespace BuildingManager.API.Application.Features.Buildings.Commands.Create;

public record CreateBuildingRequest(
    string Name,
    string BuildingType,
    string? Address,
    int? NumberOfFloors,
    int? TotalUnits,
    int? ConstructionYear,
    decimal? Latitude,
    decimal? Longitude,
    string? Amenities
);