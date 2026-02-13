using CookMate_project.Domain.Entities;

namespace CookMate_project.Domain.Repositories;

public interface IRefreshSessionRepository
{
    Task<RefreshSession?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task AddAsync(RefreshSession refreshSession, CancellationToken cancellationToken = default);
    Task RevokeFamilyAsync(Guid familyId, DateTime revokedAt, string? revokedByIp, CancellationToken cancellationToken = default);
    Task RevokeAllForUserAsync(Guid userId, DateTime revokedAt, string? revokedByIp, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
