using CookMate_project.Domain.Entities;
using CookMate_project.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CookMate_project.Infrastructure.Persistence.Repositories;

public class RefreshSessionRepository(AppDbContext dbContext) : IRefreshSessionRepository
{
    public Task<RefreshSession?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
        => dbContext.RefreshSessions.SingleOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

    public Task AddAsync(RefreshSession refreshSession, CancellationToken cancellationToken = default)
        => dbContext.RefreshSessions.AddAsync(refreshSession, cancellationToken).AsTask();

    public Task RevokeFamilyAsync(Guid familyId, DateTime revokedAt, string? revokedByIp, CancellationToken cancellationToken = default)
        => dbContext.RefreshSessions
            .Where(x => x.FamilyId == familyId && x.RevokedAt == null)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.RevokedAt, revokedAt)
                .SetProperty(x => x.RevokedByIp, revokedByIp), cancellationToken);

    public Task RevokeAllForUserAsync(Guid userId, DateTime revokedAt, string? revokedByIp, CancellationToken cancellationToken = default)
        => dbContext.RefreshSessions
            .Where(x => x.UserId == userId && x.RevokedAt == null)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.RevokedAt, revokedAt)
                .SetProperty(x => x.RevokedByIp, revokedByIp), cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);
}
