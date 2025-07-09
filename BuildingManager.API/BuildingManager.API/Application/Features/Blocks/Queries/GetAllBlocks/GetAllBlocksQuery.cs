using MediatR;
using System;
using System.Collections.Generic;
using BuildingManager.API.Application.Features.Blocks.Queries.GetBlockById; // For BlockResponseDto

namespace BuildingManager.API.Application.Features.Blocks.Queries.GetAllBlocks
{
    public class GetAllBlocksQuery : IRequest<GetAllBlocksResponse>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public Guid? ParentComplexPublicId { get; set; } // Filter by parent complex
        // Add other filter parameters like BlockType if needed
    }

    public class GetAllBlocksResponse
    {
        public List<BlockResponseDto> Items { get; set; } = new List<BlockResponseDto>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)System.Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
