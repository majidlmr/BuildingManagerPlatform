// File: Application/Features/Buildings/Queries/GetBuildingDetails/GetBuildingDetailsQuery.cs
using MediatR;
namespace BuildingManager.API.Application.Features.Buildings.Queries.GetBuildingDetails;

// این Query شناسه ساختمان را می‌گیرد و یک BuildingDetailsDto را برمی‌گرداند
public record GetBuildingDetailsQuery(int BuildingId, int RequestingUserId) : IRequest<BuildingDetailsDto>;