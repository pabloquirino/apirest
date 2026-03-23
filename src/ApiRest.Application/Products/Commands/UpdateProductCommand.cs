using ApiRest.Application.DTOs;
using ApiRest.Domain.Exceptions;
using ApiRest.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ApiRest.Application.Products.Commands;

public record UpdateProductCommand(
    Guid    Id,
    string  Name,
    string  Description,
    decimal Price) : IRequest<ProductDto>;

public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
    }
}

public class UpdateProductHandler(
    IProductRepository repo, IUnitOfWork uow)
    : IRequestHandler<UpdateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(
        UpdateProductCommand req, CancellationToken ct)
    {
        var product = await repo.GetByIdAsync(req.Id, ct)
            ?? throw new NotFoundException("Product", req.Id);

        product.Update(req.Name, req.Description, req.Price);
        repo.Update(product);
        await uow.CommitAsync(ct);

        return new ProductDto(
            product.Id, product.Name, product.Description,
            product.Price, product.Stock, product.IsActive);
    }
}