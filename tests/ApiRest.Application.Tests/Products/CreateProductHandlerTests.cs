using ApiRest.Application.Products.Commands;
using ApiRest.Domain.Entities;
using ApiRest.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ApiRest.Application.Tests.Products;

public class CreateProductHandlerTests
{
    private readonly Mock<IProductRepository> _repoMock = new();
    private readonly Mock<IUnitOfWork>        _uowMock  = new();

    private CreateProductHandler CreateHandler()
        => new(_repoMock.Object, _uowMock.Object);

    [Fact]
    public async Task Handle_ValidCommand_ShouldAddProductAndCommit()
    {
        var command = new CreateProductCommand(
            "Notebook", "Desc", 4999m, 10);

        _repoMock.Setup(r => r.AddAsync(
            It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await CreateHandler().Handle(command, default);

        result.Name.Should().Be("Notebook");
        result.Price.Should().Be(4999m);
        result.Stock.Should().Be(10);

        _repoMock.Verify(r => r.AddAsync(
            It.IsAny<Product>(), default), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRepoThrows_ShouldPropagateException()
    {
        var command = new CreateProductCommand("X", "", 1m, 1);

        _repoMock.Setup(r => r.AddAsync(
            It.IsAny<Product>(), default))
            .ThrowsAsync(new Exception("DB error"));

        await CreateHandler()
            .Invoking(h => h.Handle(command, default))
            .Should().ThrowAsync<Exception>()
            .WithMessage("DB error");

        _uowMock.Verify(u => u.CommitAsync(default), Times.Never);
    }
}