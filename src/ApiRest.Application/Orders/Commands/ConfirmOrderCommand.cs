using ApiRest.Domain.Exceptions;
using ApiRest.Domain.Interfaces;
using MediatR;

namespace ApiRest.Application.Orders.Commands;

public record ConfirmOrderCommand(Guid OrderId, Guid UserId) : IRequest;

public class ConfirmOrderHandler(
    IOrderRepository repo, IUnitOfWork uow)
    : IRequestHandler<ConfirmOrderCommand>
{
    public async Task Handle(
        ConfirmOrderCommand req, CancellationToken ct)
    {
        var order = await repo.GetByIdAsync(req.OrderId, ct)
            ?? throw new NotFoundException("Order", req.OrderId);

        if (order.UserId != req.UserId)
            throw new DomainException("You do not own this order.");

        order.Confirm();
        repo.Update(order);
        await uow.CommitAsync(ct);
    }
}
