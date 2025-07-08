using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Financials.Commands.CreateRevenue;

public class CreateRevenueCommandHandler : IRequestHandler<CreateRevenueCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    public CreateRevenueCommandHandler(IApplicationDbContext context, IUnitOfWork unitOfWork, IAuthorizationService authorizationService)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
    }

    public async Task<int> Handle(CreateRevenueCommand request, CancellationToken cancellationToken)
    {
        // ✅ بررسی دسترسی با استفاده از سیستم جدید مبتنی بر مجوز
        var canCreate = await _authorizationService.HasPermissionAsync(request.RecordedByUserId, request.BuildingId, "Revenue.Create", cancellationToken);
        if (!canCreate)
        {
            throw new ForbiddenAccessException("شما اجازه ثبت درآمد برای این ساختمان را ندارید.");
        }

        var revenue = new Revenue
        {
            BuildingId = request.BuildingId,
            Title = request.Title,
            Description = request.Description,
            Amount = request.Amount,
            RevenueDate = request.RevenueDate.ToUniversalTime(),
            Category = request.Category,
            RecordedByUserId = request.RecordedByUserId
        };

        await _context.Revenues.AddAsync(revenue, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return revenue.Id;
    }
}