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
    {
        // Pega todos os refresh tokens ativos do usuário
        var tokens = await context.RefreshTokens
            .Where(r => r.UserId == userId && !r.IsRevoked)
            .ToListAsync(ct); // materializa a lista

        // Revoga cada token individualmente
        foreach (var token in tokens)
        {
            token.Revoke();
        }

        // Salva as alterações no banco
        await context.SaveChangesAsync(ct);
    }
}