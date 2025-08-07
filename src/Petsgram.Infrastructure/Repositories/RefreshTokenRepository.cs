using Petsgram.Application.Interfaces.Auth;
using Petsgram.Domain.Entities;
using Petsgram.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Petsgram.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly PetsgramDbContext _context;

    public RefreshTokenRepository(PetsgramDbContext context)
    {
        _context = context;
    }

    public Task<RefreshToken?> FindAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.RefreshTokens.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<RefreshToken> AddAsync(RefreshToken entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var result = await _context.RefreshTokens.AddAsync(entity, cancellationToken);
        return result.Entity;
    }


    public Task UpdateAsync(RefreshToken entity, CancellationToken cancellationToken = default)
    {
        _context.RefreshTokens.Update(entity);
        return Task.CompletedTask;
    }

    public async Task RemoveAsync(int id, CancellationToken cancellationToken = default)
    {
        await _context.RefreshTokens.Where(x => x.Id == id).ExecuteDeleteAsync(cancellationToken);
    }

    public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
    }

    public Task<List<RefreshToken>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return _context.RefreshTokens.Where(rt => rt.UserId == userId).ToListAsync(cancellationToken);
    }

    public async Task RevokeTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        await _context.RefreshTokens.Where(rt => rt.Token == token).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task RevokeAllUserTokensAsync(int userId, CancellationToken cancellationToken = default)
    {
        await _context.RefreshTokens.Where(rt => rt.UserId == userId).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task CleanupExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        await _context.RefreshTokens.Where(rt => rt.ExpiresAt < DateTime.UtcNow).ExecuteDeleteAsync(cancellationToken);
    }
}