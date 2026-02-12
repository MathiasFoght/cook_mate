using CookMate_project.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CookMate_project.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

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
    }
}
