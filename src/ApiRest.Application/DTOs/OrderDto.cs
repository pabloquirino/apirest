using ApiRest.Domain.Enums;

namespace ApiRest.Application.DTOs;

public record OrderDto(
    Guid                    Id,
    Guid                    UserId,
    OrderStatus             Status,
    decimal                 Total,
    DateTime                CreatedAt,
    IReadOnlyList<OrderItemDto> Items);

public record OrderItemDto(
    Guid    ProductId,
    string  ProductName,
    int     Quantity,
    decimal UnitPrice,
    decimal Subtotal);