using CookMate_project.Domain.Entities;

namespace CookMate_project.Domain.Repositories;

public interface IUserRepository
{
    Task<bool> ExistsByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
