using ApiRest.Domain.Entities;
using ApiRest.Domain.Exceptions;
using FluentAssertions;

namespace ApiRest.Domain.Tests.Entities;

public class ProductTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateProduct()
    {
        var product = Product.Create("Notebook", "Desc", 4999m, 10);

        product.Name.Should().Be("Notebook");
        product.Price.Should().Be(4999m);
        product.Stock.Should().Be(10);
        product.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithNegativePrice_ShouldThrowDomainException()
    {
        var act = () => Product.Create("X", "", -1m, 0);

        act.Should().Throw<DomainException>()
           .WithMessage("*negative*");
    }

    [Fact]
    public void DeductStock_WithSufficientStock_ShouldReduceStock()
    {
        var product = Product.Create("X", "", 10m, 5);
        product.DeductStock(3);

        product.Stock.Should().Be(2);
    }

    [Fact]
    public void DeductStock_WithInsufficientStock_ShouldThrowDomainException()
    {
        var product = Product.Create("X", "", 10m, 2);

        var act = () => product.DeductStock(5);

        act.Should().Throw<DomainException>()
           .WithMessage("*Insufficient*");
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        var product = Product.Create("X", "", 10m, 1);
        product.Deactivate();

        product.IsActive.Should().BeFalse();
    }
}