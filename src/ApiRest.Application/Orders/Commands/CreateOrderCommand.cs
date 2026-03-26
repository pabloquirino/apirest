using ApiRest.Application.DTOs;
using ApiRest.Domain.Entities;
using ApiRest.Domain.Exceptions;
using ApiRest.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ApiRest.Application.Orders.Commands;

public record OrderItemRequest(Guid ProductId, int Quantity);

public record CreateOrderCommand(
    Guid UserId,
    List<OrderItemRequest> Items) : IRequest<OrderDto>;

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty()
            .WithMessage("Order must have at least one item.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).NotEmpty();
            item.RuleFor(i => i.Quantity).GreaterThan(0);
        });
    }
}

public class CreateOrderHandler(
    IOrderRepository   orderRepo,
    IProductRepository productRepo,
    IUnitOfWork        uow)
    : IRequestHandler<CreateOrderCommand, OrderDto>
{
    public async Task<OrderDto> Handle(
        CreateOrderCommand req, CancellationToken ct)
    {
        var order = Order.Create(req.UserId);

        foreach (var item in req.Items)
        {
            var product = await productRepo.GetByIdAsync(item.ProductId, ct)
                ?? throw new NotFoundException("Product", item.ProductId);

            if (!product.IsActive)
                throw new DomainException($"Product '{product.Name}' is inactive.");

            product.DeductStock(item.Quantity);  
            productRepo.Update(product);

            order.AddItem(product, item.Quantity);
        }

        await orderRepo.AddAsync(order, ct);
        await uow.CommitAsync(ct);

        return ToDto(order);
    }

    private static OrderDto ToDto(Order o) => new(
        o.Id, o.UserId, o.Status, o.Total, o.CreatedAt,
        o.Items.Select(i => new OrderItemDto(
            i.ProductId, string.Empty, i.Quantity,
            i.UnitPrice, i.UnitPrice * i.Quantity)).ToList());
}