using BuildingManager.API.Domain.Common;

namespace BuildingManager.API.Domain.Events;

/// <summary>
/// رویدادی که پس از پرداخت موفقیت‌آمیز یک صورتحساب منتشر می‌شود.
/// این رویداد شامل اطلاعات کلیدی برای انجام عملیات‌های بعدی است.
/// </summary>
public class InvoicePaidEvent : DomainEvent
{
    public int InvoiceId { get; }
    public int UserId { get; }
    public decimal AmountPaid { get; }

    public InvoicePaidEvent(int invoiceId, int userId, decimal amountPaid)
    {
        InvoiceId = invoiceId;
        UserId = userId;
        AmountPaid = amountPaid;
    }
}