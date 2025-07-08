// File: Application/Features/Buildings/Queries/GetMyBuildings/BuildingSummaryDto.cs
namespace BuildingManager.API.Application.Features.Buildings.Queries.GetMyBuildings;

public record BuildingSummaryDto(
    int Id,
    Guid PublicId,
    string Name,
    string BuildingType,
    string? Address
);