using ApiRest.Domain.Entities;
using ApiRest.Domain.Interfaces;
using ApiRest.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiRest.Infrastructure.Repositories;

public class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
{
    public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct)
        => context.RefreshTokens
               .Include(r => r.User)
               .FirstOrDefaultAsync(r => r.Token == token, ct);

    public async Task AddAsync(RefreshToken token, CancellationToken ct)
        => await context.RefreshTokens.AddAsync(token, ct);

    public async Task RevokeAllByUserAsync(Guid userId, CancellationToken ct)
        => await context.RefreshTokens
               .Where(r => r.UserId == userId && !r.IsRevoked)
               .ExecuteUpdateAsync(s => s.SetProperty(r => r.IsRevoked, true), ct);
}