using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Events;
using BuildingManager.API.Domain.Interfaces;
using BuildingManager.API.Domain.Enums; // Added for Enums
using MediatR;
using Microsoft.EntityFrameworkCore; // Added for FirstOrDefaultAsync
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Tickets.Commands.CreateTicket;

/// <summary>
/// پردازشگر دستور ایجاد یک تیکت جدید.
/// </summary>
public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublisher _publisher;
    private readonly IAuthorizationService _authorizationService;

    public CreateTicketCommandHandler(
        IApplicationDbContext context,
        IUnitOfWork unitOfWork,
        IPublisher publisher,
        IAuthorizationService authorizationService)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _publisher = publisher;
        _authorizationService = authorizationService;
    }

    public async Task<Guid> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        // Step 1: Security check - Is the user a member of the block?
        // Assuming IsMemberOfBuildingAsync is updated or replaced by IsMemberOfBlockAsync
        var canAccess = await _authorizationService.IsMemberOfBlockAsync(request.ReportedByUserId, request.BlockId, cancellationToken);
        if (!canAccess)
        {
            throw new ForbiddenAccessException("شما اجازه ثبت تیکت در این بلوک را ندارید.");
        }

        // Optional: Fetch the block to get ComplexId if it's not directly in the command
        var block = await _context.Blocks
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == request.BlockId, cancellationToken);

        if (block == null) // Should not happen if IsMemberOfBlockAsync passed, but good practice
        {
            throw new NotFoundException($"بلوک با شناسه {request.BlockId} یافت نشد.");
        }

        // Step 2: Create the new ticket entity
        var ticket = new Ticket
        {
            BlockId = request.BlockId, // Changed from BuildingId
            ComplexId = block.ParentComplexId, // Set ComplexId from the block
            UnitId = request.UnitId,
            ReportedByUserId = request.ReportedByUserId,
            Title = request.Title,
            Description = request.Description,
            Category = request.Category, // Now an Enum
            Priority = request.Priority, // Now an Enum
            // AttachmentUrl is removed
            Status = TicketStatus.Open, // Changed to Enum, initial status
            CreatedAt = DateTime.UtcNow, // Already set by default in entity if configured, but explicit is fine
            IsAnonymous = request.IsAnonymous
            // AssignedToUserId and ResolutionDetails will be null by default
        };

        // Step 3: Add the ticket to the database and save changes
        await _context.Tickets.AddAsync(ticket, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Step 4: Publish a domain event
        await _publisher.Publish(new TicketCreatedEvent(ticket), cancellationToken);

        // Step 5: Return the public ID of the new ticket
        return ticket.PublicId;
    }
}