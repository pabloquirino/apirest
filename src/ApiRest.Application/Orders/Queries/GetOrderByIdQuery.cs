using ApiRest.Application.DTOs;
using ApiRest.Domain.Exceptions;
using ApiRest.Domain.Interfaces;
using MediatR;

namespace ApiRest.Application.Orders.Queries;

public record GetOrderByIdQuery(Guid OrderId, Guid UserId) : IRequest<OrderDto>;

public class GetOrderByIdHandler(IOrderRepository repo)
    : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    public async Task<OrderDto> Handle(
        GetOrderByIdQuery req, CancellationToken ct)
    {
        var o = await repo.GetByIdAsync(req.OrderId, ct)
            ?? throw new NotFoundException("Order", req.OrderId);

        if (o.UserId != req.UserId)
            throw new DomainException("You do not own this order.");

        return new OrderDto(
            o.Id, o.UserId, o.Status, o.Total, o.CreatedAt,
            o.Items.Select(i => new OrderItemDto(
                i.ProductId,
                i.Product?.Name ?? string.Empty,
                i.Quantity,
                i.UnitPrice,
                i.UnitPrice * i.Quantity)).ToList());
    }
}