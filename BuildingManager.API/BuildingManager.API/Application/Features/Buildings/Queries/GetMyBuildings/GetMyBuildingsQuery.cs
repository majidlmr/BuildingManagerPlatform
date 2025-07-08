// File: Application/Features/Buildings/Queries/GetMyBuildings/GetMyBuildingsQuery.cs
using MediatR;
namespace BuildingManager.API.Application.Features.Buildings.Queries.GetMyBuildings;

// این Query شناسه مدیر را می‌گیرد و لیستی از ساختمان‌هایش را برمی‌گرداند
public record GetMyBuildingsQuery(int OwnerUserId) : IRequest<List<BuildingSummaryDto>>;