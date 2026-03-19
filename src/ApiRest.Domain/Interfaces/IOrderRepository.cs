using ApiRest.Domain.Entities;

namespace ApiRest.Domain.Interfaces;

public interface IOrderRepository
{
    Task<Order?>              GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Order>>  GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task                      AddAsync(Order order, CancellationToken ct = default);
    void                      Update(Order order);
}
