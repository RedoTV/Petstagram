using Microsoft.EntityFrameworkCore;
using Petsgram.Application.Interfaces.Auth;
using Petsgram.Domain.Entities;
using Petsgram.Infrastructure.DbContexts;

namespace Petsgram.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly PetsgramDbContext _dbContext;

    public RefreshTokenRepository(PetsgramDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RefreshToken?> FindAsync(int id)
    {
        return await _dbContext.RefreshTokens
            .AsNoTracking()
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Id == id);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _dbContext.RefreshTokens
            .AsNoTracking()
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task<IEnumerable<RefreshToken>> GetByUserIdAsync(int userId)
    {
        return await _dbContext.RefreshTokens
            .AsNoTracking()
            .Include(rt => rt.User)
            .Where(rt => rt.UserId == userId)
            .ToListAsync();
    }

    public async Task<RefreshToken> AddAsync(RefreshToken refreshToken)
    {
        await _dbContext.RefreshTokens.AddAsync(refreshToken);
        return refreshToken;
    }

    public async Task UpdateAsync(RefreshToken refreshToken)
    {
        await _dbContext.RefreshTokens
            .Where(rt => rt.Id == refreshToken.Id)
            .ExecuteUpdateAsync(setter =>
                setter.SetProperty(rt => rt.IsRevoked, refreshToken.IsRevoked)
            );
    }

    public async Task RemoveAsync(int id)
    {
        var refreshToken = await FindAsync(id);
        if (refreshToken == null)
            throw new ArgumentException($"RefreshToken with id:{id} not found");

        _dbContext.RefreshTokens.Remove(refreshToken);
    }

    public async Task RevokeTokenAsync(string token)
    {
        await _dbContext.RefreshTokens
            .Where(rt => rt.Token == token)
            .ExecuteUpdateAsync(setter =>
                setter.SetProperty(rt => rt.IsRevoked, true)
            );
    }

    public async Task RevokeAllUserTokensAsync(int userId)
    {
        await _dbContext.RefreshTokens
            .Where(rt => rt.UserId == userId)
            .ExecuteUpdateAsync(setter =>
                setter.SetProperty(rt => rt.IsRevoked, true)
            );
    }

    public async Task CleanupExpiredTokensAsync()
    {
        var expiredTokens = await _dbContext.RefreshTokens
            .Where(rt => rt.ExpiresAt < DateTime.UtcNow)
            .ToListAsync();

        _dbContext.RefreshTokens.RemoveRange(expiredTokens);
        await _dbContext.SaveChangesAsync();
    }
}