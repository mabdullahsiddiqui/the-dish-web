using Microsoft.EntityFrameworkCore;
using UserEntity = TheDish.User.Domain.Entities.User;

namespace TheDish.User.Infrastructure.Data;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    public DbSet<UserEntity> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("users");

        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
            entity.Property(u => u.PasswordHash).HasMaxLength(255);
            entity.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(u => u.LastName).IsRequired().HasMaxLength(100);
            entity.Property(u => u.ExternalProviderId).HasMaxLength(255);
            entity.Property(u => u.ExternalProviderEmail).HasMaxLength(255);

            entity.Property(u => u.ExternalProvider)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(u => u.Reputation).HasDefaultValue(0);
            entity.Property(u => u.ReviewCount).HasDefaultValue(0);
            entity.Property(u => u.IsVerified).HasDefaultValue(false);
            entity.Property(u => u.JoinDate).IsRequired();

            // Password reset fields
            entity.Property(u => u.PasswordResetCode).HasMaxLength(10);
            entity.Property(u => u.PasswordResetCodeExpiry);

            // Indexes
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => new { u.ExternalProviderId, u.ExternalProvider });
        });
    }
}

