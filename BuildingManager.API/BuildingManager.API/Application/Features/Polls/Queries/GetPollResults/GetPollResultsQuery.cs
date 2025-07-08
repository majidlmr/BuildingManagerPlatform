using MediatR;

namespace BuildingManager.API.Application.Features.Polls.Queries.GetPollResults;

public record GetPollResultsQuery(int PollId, int RequestingUserId) : IRequest<PollResultDto>;