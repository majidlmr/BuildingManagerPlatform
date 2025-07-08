using MediatR;
using System;

namespace BuildingManager.API.Application.Features.Residents.Commands.AssignResident;

public record AssignResidentCommand(
    int UnitId,
    string ResidentPhoneNumber,
    string ResidentFullName,
    DateTime StartDate,
    DateTime? EndDate,
    int RequestingUserId
) : IRequest<int>;