using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Roles.Commands.DeleteRole;

public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuthorizationService _authorizationService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRoleCommandHandler(IApplicationDbContext context, IAuthorizationService authorizationService, IUnitOfWork unitOfWork)
    {
        _context = context;
        _authorizationService = authorizationService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var canDelete = await _authorizationService.HasPermissionAsync(request.RequestingUserId, request.BuildingId, "Role.Delete", cancellationToken);
        if (!canDelete) throw new ForbiddenAccessException("شما اجازه حذف نقش در این ساختمان را ندارید.");

        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == request.RoleId && r.BuildingId == request.BuildingId, cancellationToken);
        if (role == null) throw new NotFoundException("نقش مورد نظر یافت نشد.");

        if (role.Name == "Owner") throw new ValidationException("امکان حذف نقش 'Owner' وجود ندارد.");

        _context.Roles.Remove(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}