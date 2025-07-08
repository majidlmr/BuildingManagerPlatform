using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Vehicles.Commands.CreateVehicle;

/// <summary>
/// پردازشگر دستور ثبت وسیله نقلیه جدید.
/// </summary>
public class CreateVehicleCommandHandler : IRequestHandler<CreateVehicleCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public CreateVehicleCommandHandler(IApplicationDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// منطق اصلی ثبت خودروی جدید را مدیریت می‌کند.
    /// </summary>
    public async Task<int> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = new Vehicle
        {
            UserId = request.UserId,
            LicensePlate = request.LicensePlate.ToUpper(), // پلاک را با حروف بزرگ ذخیره می‌کنیم
            Model = request.Model,
            Color = request.Color,
            Description = request.Description
        };

        await _context.Vehicles.AddAsync(vehicle, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return vehicle.Id;
    }
}