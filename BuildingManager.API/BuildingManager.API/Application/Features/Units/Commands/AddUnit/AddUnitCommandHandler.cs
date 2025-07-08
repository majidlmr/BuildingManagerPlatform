using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingUnit = BuildingManager.API.Domain.Entities.Unit;

namespace BuildingManager.API.Application.Features.Units.Commands.AddUnit;

public class AddUnitCommandHandler : IRequestHandler<AddUnitCommand, int>
{
    private readonly IUnitRepository _unitRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    public AddUnitCommandHandler(IUnitRepository unitRepository, IUnitOfWork unitOfWork, IAuthorizationService authorizationService)
    {
        _unitRepository = unitRepository;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
    }

    public async Task<int> Handle(AddUnitCommand request, CancellationToken cancellationToken)
    {
        // ✅ بررسی دسترسی با استفاده از سیستم جدید مبتنی بر مجوز "Unit.Create"
        var canCreate = await _authorizationService.HasPermissionAsync(request.RequestingUserId, request.BuildingId, "Unit.Create", cancellationToken);
        if (!canCreate)
        {
            throw new ForbiddenAccessException("شما اجازه افزودن واحد به این ساختمان را ندارید.");
        }

        var unit = new BuildingUnit
        {
            BuildingId = request.BuildingId,
            UnitNumber = request.UnitNumber,
            OwnershipStatus = request.OwnershipStatus,
            OwnerUserId = request.OwnerUserId,
            UnitType = request.UnitType,
            FloorNumber = request.FloorNumber,
            Bedrooms = request.Bedrooms,
            Area = request.Area,
            PublicId = Guid.NewGuid()
        };

        await _unitRepository.AddAsync(unit);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return unit.Id;
    }
}