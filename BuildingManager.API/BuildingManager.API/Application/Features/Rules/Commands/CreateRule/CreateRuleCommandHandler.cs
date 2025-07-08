using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Rules.Commands.CreateRule;

public class CreateRuleCommandHandler : IRequestHandler<CreateRuleCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    public CreateRuleCommandHandler(IApplicationDbContext context, IUnitOfWork unitOfWork, IAuthorizationService authorizationService)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
    }

    public async Task<int> Handle(CreateRuleCommand request, CancellationToken cancellationToken)
    {
        // ✅ بررسی دسترسی با استفاده از سیستم جدید مبتنی بر مجوز
        var canCreate = await _authorizationService.HasPermissionAsync(request.CreatedByUserId, request.BuildingId, "Rule.Create", cancellationToken);
        if (!canCreate)
        {
            throw new ForbiddenAccessException("شما اجازه افزودن قانون به این ساختمان را ندارید.");
        }

        var rule = new BuildingRule
        {
            BuildingId = request.BuildingId,
            Title = request.Title,
            Content = request.Content,
            CreatedByUserId = request.CreatedByUserId
        };

        await _context.BuildingRules.AddAsync(rule, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return rule.Id;
    }
}