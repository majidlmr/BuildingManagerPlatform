using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Roles.Commands.UpdateRole;

public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuthorizationService _authorizationService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRoleCommandHandler(IApplicationDbContext context, IAuthorizationService authorizationService, IUnitOfWork unitOfWork)
    {
        _context = context;
        _authorizationService = authorizationService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var canUpdate = await _authorizationService.HasPermissionAsync(request.RequestingUserId, request.BuildingId, "Role.Update", cancellationToken);
        if (!canUpdate) throw new ForbiddenAccessException("شما اجازه ویرایش نقش در این ساختمان را ندارید.");

        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == request.RoleId && r.BuildingId == request.BuildingId, cancellationToken);
        if (role == null) throw new NotFoundException("نقش مورد نظر یافت نشد.");

        // بررسی عدم تکراری بودن نام جدید
        var nameExists = await _context.Roles.AnyAsync(r => r.BuildingId == request.BuildingId && r.Name == request.NewName && r.Id != request.RoleId, cancellationToken);
        if (nameExists) throw new ValidationException($"نقشی با نام '{request.NewName}' از قبل وجود دارد.");

        role.Name = request.NewName;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}