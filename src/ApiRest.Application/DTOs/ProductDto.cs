namespace ApiRest.Application.DTOs;

public record ProductDto(
    Guid    Id,
    string  Name,
    string  Description,
    decimal Price,
    int     Stock,
    bool    IsActive);