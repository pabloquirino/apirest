using ApiRest.Domain.Exceptions;

namespace ApiRest.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid    OrderId   { get; private set; }
    public Guid    ProductId { get; private set; }
    public int     Quantity  { get; private set; }
    public decimal UnitPrice { get; private set; }

    public Product Product { get; private set; } = null!;

    private OrderItem() { }

    internal static OrderItem Create(Guid orderId, Guid productId,
                                       int quantity, decimal unitPrice)
        => new()
        {
            OrderId   = orderId,
            ProductId = productId,
            Quantity  = quantity,
            UnitPrice = unitPrice
        };

    internal void IncreaseQuantity(int extra)
    {
        if (extra <= 0) throw new DomainException("Extra quantity must be positive.");
        Quantity += extra;
    }
}