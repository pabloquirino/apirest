using ApiRest.Domain.Entities;

namespace ApiRest.Domain.Interfaces;

public interface IProductRepository
{
    Task<Product?>        GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Product>> GetAllActiveAsync(CancellationToken ct = default);
    Task                  AddAsync(Product product, CancellationToken ct = default);
    void                  Update(Product product);
    void                  Delete(Product product);
}
