namespace BuildingManager.API.Domain.Enums
{
    public enum TicketStatus
    {
        Open,
        InProgress,
        PendingUserInput, // منتظر اطلاعات بیشتر از کاربر
        OnHold,           // در انتظار (مثلاً قطعه یا تصمیم)
        Resolved,         // حل شده (نیاز به تایید کاربر برای بستن)
        Closed,           // بسته شده
        Cancelled,        // لغو شده
        Reopened          // بازگشایی شده
    }
}
