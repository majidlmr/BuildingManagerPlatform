using MediatR;
using System;

namespace BuildingManager.API.Application.Features.Buildings.Commands.Create;

/// <summary>
/// دستوری برای ایجاد یک ساختمان یا بلوک جدید.
/// </summary>
public record CreateBuildingCommand(
    string Name,
    string BuildingType,
    string? Address,
    int? NumberOfFloors,
    int? TotalUnits,
    int? ConstructionYear,
    decimal? Latitude,
    decimal? Longitude,
    string? Amenities,
    int OwnerUserId, // کاربری که ساختمان/بلوک را ایجاد می‌کند
    int? ParentComplexId = null // شناسه والد (مجتمع) که این بلوک به آن تعلق دارد
) : IRequest<int>;