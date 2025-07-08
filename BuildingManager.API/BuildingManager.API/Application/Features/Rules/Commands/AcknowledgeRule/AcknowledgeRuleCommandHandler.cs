using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Rules.Commands.AcknowledgeRule;

/// <summary>
/// پردازشگر دستور تایید یک قانون توسط کاربر.
/// </summary>
public class AcknowledgeRuleCommandHandler : IRequestHandler<AcknowledgeRuleCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public AcknowledgeRuleCommandHandler(IApplicationDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// منطق اصلی ثبت تاییدیه را مدیریت می‌کند.
    /// </summary>
    /// <exception cref="Exception">در صورتی که قانون یافت نشود یا کاربر قبلاً آن را تایید کرده باشد.</exception>
    public async Task Handle(AcknowledgeRuleCommand request, CancellationToken cancellationToken)
    {
        // بررسی وجود قانون
        var ruleExists = await _context.BuildingRules.AnyAsync(r => r.Id == request.RuleId, cancellationToken);
        if (!ruleExists)
        {
            throw new Exception("قانون مورد نظر یافت نشد.");
        }

        // بررسی اینکه آیا کاربر قبلاً این قانون را تایید کرده است یا خیر
        var alreadyAcknowledged = await _context.RuleAcknowledgments
            .AnyAsync(a => a.RuleId == request.RuleId && a.UserId == request.UserId, cancellationToken);

        if (alreadyAcknowledged)
        {
            // اگر کاربر قبلاً تایید کرده، خطایی رخ نمی‌دهد، صرفاً عملیات جدیدی انجام نمی‌شود.
            // در سناریوهای دیگر می‌توان یک خطا پرتاب کرد.
            return;
        }

        // ایجاد و ثبت رکورد تاییدیه جدید
        var acknowledgment = new RuleAcknowledgment
        {
            RuleId = request.RuleId,
            UserId = request.UserId
        };

        await _context.RuleAcknowledgments.AddAsync(acknowledgment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}