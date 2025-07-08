using System.Collections.Generic;
namespace BuildingManager.API.Application.Features.Members.Queries.GetBuildingMembers;

public record MemberDto(int UserId, string FullName, string PhoneNumber, List<string> Roles);