using ApiRest.Domain.Exceptions;
using ApiRest.Domain.Interfaces;
using MediatR;

namespace ApiRest.Application.Orders.Commands;

public record CancelOrderCommand(Guid OrderId, Guid UserId) : IRequest;

public class CancelOrderHandler(
    IOrderRepository repo, IUnitOfWork uow)
    : IRequestHandler<CancelOrderCommand>
{
    public async Task Handle(
        CancelOrderCommand req, CancellationToken ct)
    {
        var order = await repo.GetByIdAsync(req.OrderId, ct)
            ?? throw new NotFoundException("Order", req.OrderId);

        if (order.UserId != req.UserId)
            throw new DomainException("You do not own this order.");

        order.Cancel();
        repo.Update(order);
        await uow.CommitAsync(ct);
    }
}