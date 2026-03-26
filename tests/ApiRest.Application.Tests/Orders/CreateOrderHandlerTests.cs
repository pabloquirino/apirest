using ApiRest.Application.Orders.Commands;
using ApiRest.Domain.Entities;
using ApiRest.Domain.Exceptions;
using ApiRest.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ApiRest.Application.Tests.Orders;

public class CreateOrderHandlerTests
{
    private readonly Mock<IOrderRepository>   _orderRepo   = new();
    private readonly Mock<IProductRepository> _productRepo = new();
    private readonly Mock<IUnitOfWork>        _uow         = new();

    private CreateOrderHandler CreateHandler()
        => new(_orderRepo.Object, _productRepo.Object, _uow.Object);

    [Fact]
    public async Task Handle_ValidItems_ShouldCreateOrderAndDeductStock()
    {
        var product = Product.Create("Notebook", "", 1000m, 10);
        var userId  = Guid.NewGuid();

        _productRepo.Setup(r => r.GetByIdAsync(product.Id, default))
            .ReturnsAsync(product);
        _orderRepo.Setup(r => r.AddAsync(
            It.IsAny<Order>(), default))
            .Returns(Task.CompletedTask);
        _uow.Setup(u => u.CommitAsync(default)).ReturnsAsync(1);

        var command = new CreateOrderCommand(
            userId, [new OrderItemRequest(product.Id, 3)]);

        var result = await CreateHandler().Handle(command, default);

        result.Total.Should().Be(3000m);
        product.Stock.Should().Be(7); // estoque deduzido
        _uow.Verify(u => u.CommitAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_ProductNotFound_ShouldThrowNotFoundException()
    {
        _productRepo.Setup(r => r.GetByIdAsync(
            It.IsAny<Guid>(), default))
            .ReturnsAsync((Product?)null);

        var command = new CreateOrderCommand(
            Guid.NewGuid(), [new OrderItemRequest(Guid.NewGuid(), 1)]);

        await CreateHandler()
            .Invoking(h => h.Handle(command, default))
            .Should().ThrowAsync<NotFoundException>();

        _uow.Verify(u => u.CommitAsync(default), Times.Never);
    }

    [Fact]
    public async Task Handle_InsufficientStock_ShouldThrowDomainException()
    {
        var product = Product.Create("X", "", 100m, 1); // stock=1

        _productRepo.Setup(r => r.GetByIdAsync(product.Id, default))
            .ReturnsAsync(product);

        var command = new CreateOrderCommand(
            Guid.NewGuid(), [new OrderItemRequest(product.Id, 5)]); // pede 5

        await CreateHandler()
            .Invoking(h => h.Handle(command, default))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("*Insufficient*");
    }
}