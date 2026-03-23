using ApiRest.Application.DTOs;
using ApiRest.Domain.Interfaces;
using MediatR;

namespace ApiRest.Application.Orders.Queries;

public record GetMyOrdersQuery(Guid UserId) : IRequest<IEnumerable<OrderDto>>;

public class GetMyOrdersHandler(IOrderRepository repo)
    : IRequestHandler<GetMyOrdersQuery, IEnumerable<OrderDto>>
{
    public async Task<IEnumerable<OrderDto>> Handle(
        GetMyOrdersQuery req, CancellationToken ct)
    {
        var orders = await repo.GetByUserIdAsync(req.UserId, ct);

        return orders.Select(o => new OrderDto(
            o.Id, o.UserId, o.Status, o.Total, o.CreatedAt,
            o.Items.Select(i => new OrderItemDto(
                i.ProductId,
                i.Product?.Name ?? string.Empty,
                i.Quantity,
                i.UnitPrice,
                i.UnitPrice * i.Quantity)).ToList()));
    }
}