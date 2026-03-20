using ApiRest.Domain.Entities;
using ApiRest.Domain.Interfaces;
using ApiRest.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiRest.Infrastructure.Repositories;

public class ProductRepository(AppDbContext context) : IProductRepository
{
    public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct)
        => context.Products.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IEnumerable<Product>> GetAllActiveAsync(CancellationToken ct)
        => await context.Products
               .Where(p => p.IsActive)
               .OrderBy(p => p.Name)
               .ToListAsync(ct);

    public async Task AddAsync(Product product, CancellationToken ct)
        => await context.Products.AddAsync(product, ct);

    public void Update(Product product) => context.Products.Update(product);
    public void Delete(Product product) => context.Products.Remove(product);
}