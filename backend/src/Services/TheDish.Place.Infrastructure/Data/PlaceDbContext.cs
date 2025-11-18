using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using TheDish.Place.Domain.Entities;
using PlaceEntity = TheDish.Place.Domain.Entities.Place;

namespace TheDish.Place.Infrastructure.Data;

public class PlaceDbContext : DbContext
{
    public PlaceDbContext(DbContextOptions<PlaceDbContext> options) : base(options)
    {
    }

            public DbSet<PlaceEntity> Places { get; set; }
    public DbSet<PlacePhoto> PlacePhotos { get; set; }
    public DbSet<MenuItem> MenuItems { get; set; }
    public DbSet<DietaryCertification> DietaryCertifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("places");
        modelBuilder.HasPostgresExtension("postgis");

        // Place entity configuration
        modelBuilder.Entity<PlaceEntity>(entity =>
        {
            entity.ToTable("Places");
            entity.HasKey(p => p.Id);
            
            entity.Property(p => p.Name).IsRequired().HasMaxLength(255);
            entity.Property(p => p.Address).IsRequired();
            entity.Property(p => p.Location)
                .HasColumnType("geography(Point, 4326)")
                .IsRequired();
            
            entity.Property(p => p.Phone).HasMaxLength(50);
            entity.Property(p => p.Website).HasMaxLength(500);
            entity.Property(p => p.Email).HasMaxLength(255);
            
            entity.Property(p => p.PriceRange)
                .IsRequired()
                .HasDefaultValue(1);
            
            // JSONB columns for dietary tags and trust scores
            entity.Property(p => p.CuisineTypes)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
            
            entity.Property(p => p.DietaryTags)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, bool>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, bool>());
            
            entity.Property(p => p.TrustScores)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, int>());
            
            entity.Property(p => p.HoursOfOperation)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => v != null ? System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null) : null,
                    v => v != null ? System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null) : null);
            
            entity.Property(p => p.Amenities)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
            
            entity.Property(p => p.AverageRating)
                .HasPrecision(3, 2)
                .HasDefaultValue(0);
            
            entity.Property(p => p.ReviewCount)
                .HasDefaultValue(0);
            
            // Indexes
            entity.HasIndex(p => p.Location)
                .HasMethod("GIST");
            entity.HasIndex(p => p.Name);
            entity.HasIndex(p => p.ClaimedBy);
            entity.HasIndex(p => p.Status);
            entity.HasIndex(p => new { p.AverageRating, p.ReviewCount });
            
            // Navigation properties
            entity.HasMany(p => p.Photos)
                .WithOne(pp => pp.Place)
                .HasForeignKey(pp => pp.PlaceId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasMany(p => p.MenuItems)
                .WithOne(m => m.Place)
                .HasForeignKey(m => m.PlaceId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasMany(p => p.Certifications)
                .WithOne(c => c.Place)
                .HasForeignKey(c => c.PlaceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // PlacePhoto entity configuration
        modelBuilder.Entity<PlacePhoto>(entity =>
        {
            entity.ToTable("PlacePhotos");
            entity.HasKey(pp => pp.Id);
            
            entity.Property(pp => pp.Url).IsRequired().HasMaxLength(1000);
            entity.Property(pp => pp.ThumbnailUrl).HasMaxLength(1000);
            entity.Property(pp => pp.Caption).HasMaxLength(500);
            
            entity.HasIndex(pp => pp.PlaceId);
            entity.HasIndex(pp => new { pp.PlaceId, pp.IsFeatured });
        });

        // MenuItem entity configuration
        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.ToTable("MenuItems");
            entity.HasKey(m => m.Id);
            
            entity.Property(m => m.Name).IsRequired().HasMaxLength(255);
            entity.Property(m => m.Description).HasMaxLength(1000);
            entity.Property(m => m.Category).HasMaxLength(100);
            entity.Property(m => m.PhotoUrl).HasMaxLength(1000);
            
            entity.Property(m => m.Price).HasPrecision(10, 2);
            
            entity.Property(m => m.DietaryTags)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
            
            entity.Property(m => m.AllergenWarnings)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
            
            entity.HasIndex(m => m.PlaceId);
            entity.HasIndex(m => m.Category);
        });

        // DietaryCertification entity configuration
        modelBuilder.Entity<DietaryCertification>(entity =>
        {
            entity.ToTable("DietaryCertifications");
            entity.HasKey(c => c.Id);
            
            entity.Property(c => c.DietaryType).IsRequired().HasMaxLength(50);
            entity.Property(c => c.CertificationLevel).HasMaxLength(50);
            entity.Property(c => c.CertificatePhotoUrl).HasMaxLength(1000);
            entity.Property(c => c.CertificateNumber).HasMaxLength(100);
            entity.Property(c => c.CertifyingAuthority).HasMaxLength(255);
            entity.Property(c => c.RejectionReason).HasMaxLength(1000);
            
            entity.Property(c => c.TrustScore).HasPrecision(5, 2);
            
            entity.HasIndex(c => new { c.PlaceId, c.DietaryType });
            entity.HasIndex(c => c.VerificationStatus);
            entity.HasIndex(c => c.TrustScore);
            entity.HasIndex(c => c.ExpiryDate);
        });
    }
}

