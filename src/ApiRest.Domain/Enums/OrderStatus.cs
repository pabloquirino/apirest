namespace ApiRest.Domain.Enums
{
    public enum OrderStatus
    {
        None = 0,
        Pending = 1,
        Confirmed = 2,
        Shipped = 3,
        Delivered = 4,
        Cancelled = 5
    }
}