using MediatR;
using System.Collections.Generic;

namespace BuildingManager.API.Application.Features.Announcements.Queries.GetAnnouncements;

public record GetAnnouncementsQuery(int BuildingId, int RequestingUserId) : IRequest<List<AnnouncementDto>>;