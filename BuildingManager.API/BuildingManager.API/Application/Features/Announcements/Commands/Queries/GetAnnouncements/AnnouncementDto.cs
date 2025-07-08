using System;

namespace BuildingManager.API.Application.Features.Announcements.Queries.GetAnnouncements;

public record AnnouncementDto(
    int Id,
    string Title,
    string Content,
    string CreatedBy,
    DateTime CreatedAt,
    bool IsPinned
);