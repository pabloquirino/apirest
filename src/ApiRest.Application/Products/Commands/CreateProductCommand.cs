using ApiRest.Application.DTOs;
using ApiRest.Domain.Entities;
using ApiRest.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ApiRest.Application.Products.Commands;

public record CreateProductCommand(
    string  Name,
    string  Description,
    decimal Price,
    int     Stock) : IRequest<ProductDto>;

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
    }
}

public class CreateProductHandler(
    IProductRepository repo, IUnitOfWork uow)
    : IRequestHandler<CreateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(
        CreateProductCommand req, CancellationToken ct)
    {
        var product = Product.Create(req.Name, req.Description, req.Price, req.Stock);
        await repo.AddAsync(product, ct);
        await uow.CommitAsync(ct);
        return ToDto(product);
    }

    private static ProductDto ToDto(Product p) =>
        new(p.Id, p.Name, p.Description, p.Price, p.Stock, p.IsActive);
}