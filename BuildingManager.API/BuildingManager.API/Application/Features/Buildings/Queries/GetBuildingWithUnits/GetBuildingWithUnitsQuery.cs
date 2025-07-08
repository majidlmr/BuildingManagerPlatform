// File: Application/Features/Buildings/Queries/GetBuildingWithUnits/GetBuildingWithUnitsQuery.cs
using MediatR;
namespace BuildingManager.API.Application.Features.Buildings.Queries.GetBuildingWithUnits;

// این Query شناسه ساختمان را می‌گیرد و یک BuildingDetailsDto را برمی‌گرداند
public record GetBuildingWithUnitsQuery(int BuildingId, int RequestingUserId) : IRequest<BuildingDetailsDto>;