using ApiRest.Domain.Entities;
using ApiRest.Domain.Interfaces;
using ApiRest.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiRest.Infrastructure.Repositories;

public class OrderRepository(AppDbContext context) : IOrderRepository
{
    public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct)
        => context.Orders
               .Include(o => o.Items)
                   .ThenInclude(i => i.Product)
               .FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId, CancellationToken ct)
        => await context.Orders
               .Include(o => o.Items)
               .Where(o => o.UserId == userId)
               .OrderByDescending(o => o.CreatedAt)
               .ToListAsync(ct);

    public async Task AddAsync(Order order, CancellationToken ct)
        => await context.Orders.AddAsync(order, ct);

    public void Update(Order order) => context.Orders.Update(order);
}