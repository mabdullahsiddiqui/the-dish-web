using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using TheDish.Review.Domain.Entities;
using ReviewEntity = TheDish.Review.Domain.Entities.Review;

namespace TheDish.Review.Infrastructure.Data;

public class ReviewDbContext : DbContext
{
    public ReviewDbContext(DbContextOptions<ReviewDbContext> options) : base(options)
    {
    }

    public DbSet<ReviewEntity> Reviews { get; set; }
    public DbSet<ReviewPhoto> ReviewPhotos { get; set; }
    public DbSet<ReviewHelpfulness> ReviewHelpfulness { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("reviews");
        modelBuilder.HasPostgresExtension("postgis");

        // Review entity configuration
        modelBuilder.Entity<ReviewEntity>(entity =>
        {
            entity.ToTable("Reviews");
            entity.HasKey(r => r.Id);
            
            entity.Property(r => r.UserId).IsRequired();
            entity.Property(r => r.PlaceId).IsRequired();
            entity.Property(r => r.Rating)
                .IsRequired()
                .HasDefaultValue(1);
            
            entity.Property(r => r.Text).IsRequired();
            
            entity.Property(r => r.PhotoUrls)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
            
            entity.Property(r => r.DietaryAccuracy)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, string>());
            
            entity.Property(r => r.CheckInLocation)
                .HasColumnType("geography(Point, 4326)");
            
            entity.Property(r => r.HelpfulCount).HasDefaultValue(0);
            entity.Property(r => r.NotHelpfulCount).HasDefaultValue(0);
            entity.Property(r => r.Status).HasDefaultValue(Domain.Enums.ReviewStatus.Active);
            
            // Indexes
            entity.HasIndex(r => r.PlaceId);
            entity.HasIndex(r => r.UserId);
            entity.HasIndex(r => new { r.PlaceId, r.CreatedAt });
            entity.HasIndex(r => new { r.UserId, r.CreatedAt });
            entity.HasIndex(r => r.Rating);
            entity.HasIndex(r => r.GpsVerified);
            entity.HasIndex(r => r.Status);
            
            // Unique constraint: one review per user per place
            entity.HasIndex(r => new { r.UserId, r.PlaceId }).IsUnique();
            
            // Navigation properties
            entity.HasMany(r => r.Photos)
                .WithOne(rp => rp.Review)
                .HasForeignKey(rp => rp.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasMany(r => r.HelpfulnessVotes)
                .WithOne(rh => rh.Review)
                .HasForeignKey(rh => rh.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ReviewPhoto entity configuration
        modelBuilder.Entity<ReviewPhoto>(entity =>
        {
            entity.ToTable("ReviewPhotos");
            entity.HasKey(rp => rp.Id);
            
            entity.Property(rp => rp.Url).IsRequired().HasMaxLength(1000);
            entity.Property(rp => rp.ThumbnailUrl).HasMaxLength(1000);
            entity.Property(rp => rp.Caption).HasMaxLength(500);
            
            entity.HasIndex(rp => rp.ReviewId);
        });

        // ReviewHelpfulness entity configuration
        modelBuilder.Entity<ReviewHelpfulness>(entity =>
        {
            entity.ToTable("ReviewHelpfulness");
            entity.HasKey(rh => rh.Id);
            
            // Unique constraint: one vote per user per review
            entity.HasIndex(rh => new { rh.ReviewId, rh.UserId }).IsUnique();
            
            entity.HasIndex(rh => rh.ReviewId);
            entity.HasIndex(rh => rh.UserId);
        });
    }
}

