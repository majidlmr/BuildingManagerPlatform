using MediatR;
using System;
using System.Collections.Generic;
using BuildingManager.API.Application.Features.Units.Queries.GetUnitById; // For UnitResponseDto

namespace BuildingManager.API.Application.Features.Units.Queries.GetAllUnits
{
    public class GetAllUnitsQuery : IRequest<GetAllUnitsResponse>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; } // Search in UnitNumber or Description
        public Guid? BlockPublicId { get; set; } // Required filter to get units of a specific block
        // Add other filters like UnitType if needed
    }

    public class GetAllUnitsResponse
    {
        public List<UnitResponseDto> Items { get; set; } = new List<UnitResponseDto>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)System.Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
