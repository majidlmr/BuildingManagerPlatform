using MediatR;
using System.Collections.Generic;
namespace BuildingManager.API.Application.Features.Members.Queries.GetBuildingMembers;

public record GetBuildingMembersQuery(int BuildingId, int RequestingUserId) : IRequest<List<MemberDto>>;