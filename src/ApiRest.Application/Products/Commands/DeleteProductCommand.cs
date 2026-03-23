using ApiRest.Domain.Exceptions;
using ApiRest.Domain.Interfaces;
using MediatR;

namespace ApiRest.Application.Products.Commands;

public record DeleteProductCommand(Guid Id) : IRequest;

public class DeleteProductHandler(
    IProductRepository repo, IUnitOfWork uow)
    : IRequestHandler<DeleteProductCommand>
{
    public async Task Handle(
        DeleteProductCommand req, CancellationToken ct)
    {
        var product = await repo.GetByIdAsync(req.Id, ct)
            ?? throw new NotFoundException("Product", req.Id);

        product.Deactivate(); // soft delete — mantém histórico
        repo.Update(product);
        await uow.CommitAsync(ct);
    }
}