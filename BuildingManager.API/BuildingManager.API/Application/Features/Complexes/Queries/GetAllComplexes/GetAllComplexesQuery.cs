using MediatR;
using System.Collections.Generic;
using BuildingManager.API.Application.Features.Complexes.Queries.GetComplexById; // For ComplexResponseDto

namespace BuildingManager.API.Application.Features.Complexes.Queries.GetAllComplexes
{
    public class GetAllComplexesQuery : IRequest<GetAllComplexesResponse>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; } // Optional search term for name/address
        // Add other filter parameters as needed, e.g., ComplexType
    }

    public class GetAllComplexesResponse
    {
        public List<ComplexResponseDto> Items { get; set; } = new List<ComplexResponseDto>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)System.Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
