using ApiRest.API.Extensions;
using ApiRest.Application.Orders.Commands;
using ApiRest.Application.Orders.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiRest.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // todos os endpoints exigem autenticação
public class OrdersController(ISender sender) : ControllerBase
{
    // GET api/orders — pedidos do usuário logado
    [HttpGet]
    public async Task<IActionResult> GetMyOrders(CancellationToken ct)
    {
        var userId = User.GetUserId();
        return Ok(await sender.Send(new GetMyOrdersQuery(userId), ct));
    }

    // GET api/orders/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var userId = User.GetUserId();
        return Ok(await sender.Send(new GetOrderByIdQuery(id, userId), ct));
    }

    // POST api/orders — criar pedido
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrderRequest req,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var items  = req.Items
            .Select(i => new OrderItemRequest(i.ProductId, i.Quantity))
            .ToList();

        var result = await sender.Send(
            new CreateOrderCommand(userId, items), ct);

        return CreatedAtAction(
            nameof(GetById), new { id = result.Id }, result);
    }

    // POST api/orders/{id}/confirm
    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id, CancellationToken ct)
    {
        await sender.Send(new ConfirmOrderCommand(id, User.GetUserId()), ct);
        return NoContent();
    }

    // POST api/orders/{id}/cancel
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
    {
        await sender.Send(new CancelOrderCommand(id, User.GetUserId()), ct);
        return NoContent();
    }
}

public record CreateOrderRequest(List<OrderItemInput> Items);
public record OrderItemInput(Guid ProductId, int Quantity);