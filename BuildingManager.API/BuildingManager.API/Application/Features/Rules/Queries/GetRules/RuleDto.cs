namespace BuildingManager.API.Application.Features.Rules.Queries.GetRules;

public record RuleDto(
    int Id,
    string Title,
    string Content,
    bool IsAcknowledgedByCurrentUser // آیا کاربر فعلی این قانون را تایید کرده؟
);