using AutoMapper;
using BuildingManager.API.Application.Features.Complexes.Queries.GetComplexById; // For ComplexResponseDto
using BuildingManager.API.Domain.Entities; // For Complex entity

namespace BuildingManager.API.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Complex Mappings
            CreateMap<Complex, ComplexResponseDto>();

            // Add other mappings here as needed
            // For example, if GetMyBuildingsQueryHandler returns a list of Block DTOs:
            // CreateMap<Block, BuildingSummaryDto>(); // Assuming BuildingSummaryDto is for Block
        }
    }
}
