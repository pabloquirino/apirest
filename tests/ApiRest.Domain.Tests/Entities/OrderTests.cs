using ApiRest.Domain.Entities;
using ApiRest.Domain.Enums;
using ApiRest.Domain.Exceptions;
using FluentAssertions;

namespace ApiRest.Domain.Tests.Entities;

public class OrderTests
{
    private static Product MakeProduct(int stock = 10)
        => Product.Create("Notebook", "Desc", 1000m, stock);

    [Fact]
    public void AddItem_ToPendingOrder_ShouldUpdateTotal()
    {
        var order   = Order.Create(Guid.NewGuid());
        var product = MakeProduct();

        order.AddItem(product, 2);

        order.Total.Should().Be(2000m);
        order.Items.Should().HaveCount(1);
    }

    [Fact]
    public void AddItem_SameProductTwice_ShouldIncreaseQuantity()
    {
        var order   = Order.Create(Guid.NewGuid());
        var product = MakeProduct();

        order.AddItem(product, 1);
        order.AddItem(product, 2);

        order.Items.Should().HaveCount(1);
        order.Items.First().Quantity.Should().Be(3);
        order.Total.Should().Be(3000m);
    }

    [Fact]
    public void Confirm_WithItems_ShouldSetStatusConfirmed()
    {
        var order = Order.Create(Guid.NewGuid());
        order.AddItem(MakeProduct(), 1);
        order.Confirm();

        order.Status.Should().Be(OrderStatus.Confirmed);
    }

    [Fact]
    public void Confirm_EmptyOrder_ShouldThrowDomainException()
    {
        var order = Order.Create(Guid.NewGuid());

        order.Invoking(o => o.Confirm())
             .Should().Throw<DomainException>()
             .WithMessage("*empty*");
    }

    [Fact]
    public void Cancel_DeliveredOrder_ShouldThrowDomainException()
    {
        var order = Order.Create(Guid.NewGuid());
        order.AddItem(MakeProduct(), 1);
        order.Confirm();

        // forçar status Delivered via reflection (sem expor setter)
        typeof(Order)
            .GetProperty(nameof(Order.Status))!
            .SetValue(order, OrderStatus.Delivered);

        order.Invoking(o => o.Cancel())
             .Should().Throw<DomainException>();
    }

    [Fact]
    public void AddItem_ToConfirmedOrder_ShouldThrowDomainException()
    {
        var order = Order.Create(Guid.NewGuid());
        order.AddItem(MakeProduct(), 1);
        order.Confirm();

        order.Invoking(o => o.AddItem(MakeProduct(), 1))
             .Should().Throw<DomainException>()
             .WithMessage("*non-pending*");
    }
}