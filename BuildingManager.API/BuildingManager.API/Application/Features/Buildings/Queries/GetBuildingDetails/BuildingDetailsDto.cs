// File: Application/Features/Buildings/Queries/GetBuildingDetails/BuildingDetailsDto.cs
namespace BuildingManager.API.Application.Features.Buildings.Queries.GetBuildingDetails;

public record BuildingDetailsDto(
    Guid PublicId,
    string Name,
    string BuildingType,
    string? Address,
    int? NumberOfFloors,
    int? TotalUnits,        // <-- فیلد جا افتاده
    decimal? Latitude,       // <-- فیلد جا افتاده
    decimal? Longitude,      // <-- فیلد جا افتاده
    string? Amenities,       // <-- فیلد جا افتاده
    List<UnitDto> Units
);