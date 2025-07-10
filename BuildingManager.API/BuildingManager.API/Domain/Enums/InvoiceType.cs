namespace BuildingManager.API.Domain.Enums
{
    public enum InvoiceType
    {
        BuildingCharge, // شارژ ساختمان
        ServiceFee,     // هزینه خدمات (مثلاً تعمیرات خاص واحد)
        Fine,           // جریمه
        MoveInFee,      // هزینه بدو ورود
        Other           // سایر موارد
    }
}
