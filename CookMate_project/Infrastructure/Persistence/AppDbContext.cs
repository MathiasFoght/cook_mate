using CookMate_project.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CookMate_project.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshSession> RefreshSessions => Set<RefreshSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Email).IsUnique();

            entity.Property(x => x.Email).IsRequired().HasMaxLength(320);
            entity.Property(x => x.PasswordHash).IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<RefreshSession>(entity =>
        {
            entity.ToTable("refresh_sessions");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.TokenHash).IsUnique();
            entity.HasIndex(x => x.UserId);
            entity.HasIndex(x => x.FamilyId);

            entity.Property(x => x.TokenHash).IsRequired().HasMaxLength(128);
            entity.Property(x => x.ExpiresAt).IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();

            entity.HasOne(x => x.User)
                .WithMany(x => x.RefreshSessions)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
