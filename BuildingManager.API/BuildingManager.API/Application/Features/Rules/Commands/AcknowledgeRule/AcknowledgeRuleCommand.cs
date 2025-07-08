using MediatR;

namespace BuildingManager.API.Application.Features.Rules.Commands.AcknowledgeRule;

/// <summary>
/// دستوری برای ثبت تاییدیه مطالعه و پذیرش یک قانون توسط کاربر.
/// </summary>
/// <param name="RuleId">شناسه قانونی که تایید می‌شود.</param>
/// <param name="UserId">شناسه کاربری که قانون را تایید می‌کند.</param>
public record AcknowledgeRuleCommand(int RuleId, int UserId) : IRequest; // این دستور نتیجه خاصی برنمی‌گرداند