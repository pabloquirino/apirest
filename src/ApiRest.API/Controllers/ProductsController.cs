using ApiRest.Application.Products.Commands;
using ApiRest.Application.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiRest.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(ISender sender) : ControllerBase
{
    // GET api/products — público, qualquer um pode listar
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await sender.Send(new GetProductsQuery(), ct));

    // GET api/products/{id} — público
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await sender.Send(new GetProductByIdQuery(id), ct));

    // POST api/products — somente Admin
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(
        [FromBody] CreateProductRequest req,
        CancellationToken ct)
    {
        var result = await sender.Send(
            new CreateProductCommand(
                req.Name, req.Description, req.Price, req.Stock), ct);

        return CreatedAtAction(
            nameof(GetById), new { id = result.Id }, result);
    }

    // PUT api/products/{id} — somente Admin
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateProductRequest req,
        CancellationToken ct)
    {
        var result = await sender.Send(
            new UpdateProductCommand(
                id, req.Name, req.Description, req.Price), ct);
        return Ok(result);
    }

    // DELETE api/products/{id} — somente Admin (soft delete)
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await sender.Send(new DeleteProductCommand(id), ct);
        return NoContent();
    }
}

public record CreateProductRequest(string Name, string Description, decimal Price, int Stock);
public record UpdateProductRequest(string Name, string Description, decimal Price);