using System; // Added for DateTime
using System.Collections.Generic; // Added for List
using BuildingManager.API.Domain.Enums; // Added for Enums

namespace BuildingManager.API.Application.Features.Tickets.Queries.GetTicketDetails;

public record TicketAttachmentDto( // Added basic DTO for attachments
    Guid PublicId, // Assuming TicketAttachment will also have a PublicId or we use Id
    string FileUrl,
    string? FileName,
    DateTime UploadedAt
);

public record TicketDetailsDto
{
    public Guid PublicId { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public TicketStatus Status { get; init; } // Changed to Enum
    public TicketPriority Priority { get; init; } // Changed to Enum
    public TicketCategory Category { get; init; } // Changed to Enum
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public DateTime? ResolvedAt { get; init; } // From Ticket entity

    public Guid? ComplexPublicId { get; init; } // Added
    public Guid BlockPublicId { get; init; } // Added
    public Guid? UnitPublicId { get; init; } // Changed from int? UnitId

    public string ReportedByUserName { get; init; } // Renamed from ReportedBy for clarity
    public Guid ReportedByUserPublicId { get; init; } // Added

    public string? AssignedToUserName { get; init; } // Added
    public Guid? AssignedToUserPublicId { get; init; } // Added

    public string? ResolutionDetails { get; init; } // Added
    public bool IsAnonymous { get; init; } // Added from Ticket entity

    public List<TicketAttachmentDto> Attachments { get; init; } = new List<TicketAttachmentDto>(); // Added
    public List<TicketUpdateDto> Updates { get; init; } = new List<TicketUpdateDto>(); // Assuming TicketUpdateDto exists for comments/updates
}

// Assuming a DTO for TicketUpdate exists or will be created, e.g.:
public record TicketUpdateDto(
    Guid PublicId, // Or regular Id
    string UpdateText,
    string UpdatedByUserName,
    DateTime CreatedAt,
    bool IsInternalNote
);