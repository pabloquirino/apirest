using ApiRest.Application.DTOs;
using ApiRest.Domain.Interfaces;
using MediatR;

namespace ApiRest.Application.Products.Queries;

public record GetProductsQuery : IRequest<IEnumerable<ProductDto>>;

public class GetProductsHandler(IProductRepository repo)
    : IRequestHandler<GetProductsQuery, IEnumerable<ProductDto>>
{
    public async Task<IEnumerable<ProductDto>> Handle(
        GetProductsQuery _, CancellationToken ct)
    {
        var products = await repo.GetAllActiveAsync(ct);
        return products.Select(p =>
            new ProductDto(p.Id, p.Name, p.Description, p.Price, p.Stock, p.IsActive));
    }
}