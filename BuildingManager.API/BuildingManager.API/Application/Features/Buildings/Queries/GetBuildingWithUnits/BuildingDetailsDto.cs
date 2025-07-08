// File: Application/Features/Buildings/Queries/GetBuildingWithUnits/BuildingDetailsDto.cs
namespace BuildingManager.API.Application.Features.Buildings.Queries.GetBuildingWithUnits;

public record BuildingDetailsDto(
    Guid PublicId,
    string Name,
    string BuildingType,
    string? Address,
    int? NumberOfFloors,
    int? TotalUnits,
    decimal? Latitude,
    decimal? Longitude,
    string? Amenities,
    List<UnitDto> Units // لیستی از واحدهای این ساختمان
);