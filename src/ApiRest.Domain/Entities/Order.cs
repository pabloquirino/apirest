using ApiRest.Domain.Enums;
using ApiRest.Domain.Exceptions;

namespace ApiRest.Domain.Entities;

public class Order : BaseEntity
{
    public Guid        UserId { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public decimal     Total  { get; private set; }

    private readonly List<OrderItem> _items = [];
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public User User { get; private set; } = null!;

    private Order() { }

    public static Order Create(Guid userId)
        => new() { UserId = userId };

    public void AddItem(Product product, int quantity)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Cannot modify a non-pending order.");
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        var existing = _items.FirstOrDefault(i => i.ProductId == product.Id);
        if (existing is not null)
            existing.IncreaseQuantity(quantity);
        else
            _items.Add(OrderItem.Create(Id, product.Id, quantity, product.Price));

        RecalculateTotal();
        Touch();
    }

    public void Confirm()
    {
        if (!_items.Any())
            throw new DomainException("Cannot confirm an empty order.");
        if (Status != OrderStatus.Pending)
            throw new DomainException("Order is not pending.");
        Status = OrderStatus.Confirmed; Touch();
    }

    public void Cancel()
    {
        if (Status is OrderStatus.Delivered or OrderStatus.Shipped)
            throw new DomainException("Cannot cancel an order already shipped or delivered.");
        Status = OrderStatus.Cancelled; Touch();
    }

    private void RecalculateTotal()
        => Total = _items.Sum(i => i.UnitPrice * i.Quantity);
}