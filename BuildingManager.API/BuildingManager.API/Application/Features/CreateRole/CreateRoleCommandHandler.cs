using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;

namespace BuildingManager.API.Application.Features.Roles.Commands.CreateRole;

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    public CreateRoleCommandHandler(IApplicationDbContext context, IUnitOfWork unitOfWork, IAuthorizationService authorizationService)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
    }

    public async Task<int> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        // گام ۱: بررسی اینکه آیا کاربر دسترسی لازم برای ایجاد نقش را دارد یا خیر
        var canCreateRole = await _authorizationService.HasPermissionAsync(request.RequestingUserId, request.BuildingId, "Role.Create", cancellationToken);
        if (!canCreateRole)
        {
            throw new ForbiddenAccessException("شما اجازه ایجاد نقش جدید در این ساختمان را ندارید.");
        }

        // گام ۲: بررسی اینکه آیا نقشی با همین نام از قبل در این ساختمان وجود دارد یا خیر
        var roleExists = await _context.Roles
            .AnyAsync(r => r.BuildingId == request.BuildingId && r.Name == request.RoleName, cancellationToken);
        if (roleExists)
        {
            throw new ValidationException($"نقشی با نام '{request.RoleName}' در این ساختمان از قبل وجود دارد.");
        }

        // گام ۳: ایجاد و ذخیره نقش جدید
        var role = new Role
        {
            BuildingId = request.BuildingId,
            Name = request.RoleName
        };

        await _context.Roles.AddAsync(role, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return role.Id;
    }
}