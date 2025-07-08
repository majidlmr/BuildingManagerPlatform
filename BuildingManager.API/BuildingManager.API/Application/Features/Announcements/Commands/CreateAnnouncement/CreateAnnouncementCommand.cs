using MediatR;
using System;

namespace BuildingManager.API.Application.Features.Announcements.Commands.CreateAnnouncement;

public record CreateAnnouncementCommand(
    int BuildingId,
    string Title,
    string Content,
    DateTime? ExpiresAt,
    bool IsPinned,
    int CreatedByUserId
) : IRequest<int>;