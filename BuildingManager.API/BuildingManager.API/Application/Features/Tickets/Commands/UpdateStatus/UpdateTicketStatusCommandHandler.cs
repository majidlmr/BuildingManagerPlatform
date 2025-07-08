using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Tickets.Commands.UpdateStatus;

public class UpdateTicketStatusCommandHandler : IRequestHandler<UpdateTicketStatusCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    public UpdateTicketStatusCommandHandler(IApplicationDbContext context, IUnitOfWork unitOfWork, IAuthorizationService authorizationService)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
    }

    // The return type is changed from Task<int> to just Task
    public async Task Handle(UpdateTicketStatusCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _context.Tickets
            .FirstOrDefaultAsync(t => t.PublicId == request.PublicId, cancellationToken);

        if (ticket == null)
        {
            throw new NotFoundException("The specified ticket was not found.");
        }

        var canUpdate = await _authorizationService.HasPermissionAsync(request.RequestingUserId, ticket.BuildingId, "Ticket.UpdateStatus", cancellationToken);
        if (!canUpdate)
        {
            throw new ForbiddenAccessException("You do not have permission to change the status of this ticket.");
        }

        ticket.Status = request.NewStatus;
        ticket.UpdatedAt = DateTime.UtcNow;

        if (request.NewStatus.Equals("Resolved", StringComparison.OrdinalIgnoreCase) ||
            request.NewStatus.Equals("Closed", StringComparison.OrdinalIgnoreCase))
        {
            ticket.ResolvedAt = DateTime.UtcNow;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}