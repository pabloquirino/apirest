using ApiRest.Application.DTOs;
using ApiRest.Domain.Exceptions;
using ApiRest.Domain.Interfaces;
using MediatR;

namespace ApiRest.Application.Products.Queries;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;

public class GetProductByIdHandler(IProductRepository repo)
    : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    public async Task<ProductDto> Handle(
        GetProductByIdQuery req, CancellationToken ct)
    {
        var p = await repo.GetByIdAsync(req.Id, ct)
            ?? throw new NotFoundException("Product", req.Id);

        return new ProductDto(p.Id, p.Name, p.Description, p.Price, p.Stock, p.IsActive);
    }
}