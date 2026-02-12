using CookMate_project.Domain.Entities;
using CookMate_project.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CookMate_project.Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public Task<bool> ExistsByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
        => dbContext.Users.AnyAsync(x => x.Email == normalizedEmail, cancellationToken);

    public Task<User?> GetByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
        => dbContext.Users.SingleOrDefaultAsync(x => x.Email == normalizedEmail, cancellationToken);

    public Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => dbContext.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Id == userId, cancellationToken);

    public Task AddAsync(User user, CancellationToken cancellationToken = default)
        => dbContext.Users.AddAsync(user, cancellationToken).AsTask();

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);
}
