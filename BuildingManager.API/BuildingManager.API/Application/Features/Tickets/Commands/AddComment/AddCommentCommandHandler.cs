using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Tickets.Commands.AddComment;

public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService; // 👈 سرویس دسترسی

    public AddCommentCommandHandler(IApplicationDbContext context, IUnitOfWork unitOfWork, IAuthorizationService authorizationService)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService; // 👈 مقداردهی
    }

    public async Task<int> Handle(AddCommentCommand request, CancellationToken cancellationToken)
    {
        // ✅ TODO تکمیل شد: بررسی دسترسی کاربر برای کامنت گذاشتن
        var canAccess = await _authorizationService.CanAccessTicketAsync(request.UserId, request.TicketPublicId, cancellationToken);
        if (!canAccess)
        {
            throw new ForbiddenAccessException("شما اجازه افزودن کامنت به این تیکت را ندارید.");
        }

        var ticket = await _context.Tickets
            .FirstOrDefaultAsync(t => t.PublicId == request.TicketPublicId, cancellationToken);

        if (ticket == null)
        {
            throw new NotFoundException("تیکت یافت نشد.");
        }

        var ticketUpdate = new TicketUpdate
        {
            TicketId = ticket.Id,
            Comment = request.Comment,
            UpdateByUserId = request.UserId,
            CreatedAt = DateTime.UtcNow
        };

        ticket.UpdatedAt = DateTime.UtcNow;

        await _context.TicketUpdates.AddAsync(ticketUpdate, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ticketUpdate.Id;
    }
}