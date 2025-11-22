using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TheDish.Place.Infrastructure.Data;
using TheDish.Review.Infrastructure.Data;
using TheDish.Review.Domain.Enums;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var configuration = builder.Build();

Console.WriteLine("=== Update Place Ratings Tool ===\n");

// Setup Review DbContext
var reviewConnectionString = configuration.GetConnectionString("ReviewDb") 
    ?? configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Port=5432;Database=thedish;Username=thedish;Password=thedish_dev_password";

var reviewOptions = new DbContextOptionsBuilder<ReviewDbContext>()
    .UseNpgsql(reviewConnectionString, o => o.UseNetTopologySuite())
    .Options;

using var reviewContext = new ReviewDbContext(reviewOptions);

// Setup Place DbContext
var placeConnectionString = configuration.GetConnectionString("PlaceDb")
    ?? configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Port=5432;Database=thedish;Username=thedish;Password=thedish_dev_password";

var placeOptions = new DbContextOptionsBuilder<PlaceDbContext>()
    .UseNpgsql(placeConnectionString, o => o.UseNetTopologySuite())
    .Options;

using var placeContext = new PlaceDbContext(placeOptions);

try
{
    Console.WriteLine("Fetching all places...");
    var places = await placeContext.Places.ToListAsync();
    Console.WriteLine($"Found {places.Count} places.\n");

    if (places.Count == 0)
    {
        Console.WriteLine("No places found. Exiting...");
        return;
    }

    Console.WriteLine("Calculating ratings from reviews...\n");
    var updatedCount = 0;
    var skippedCount = 0;

    foreach (var place in places)
    {
        // Get review count and average rating for this place
        var reviewCount = await reviewContext.Reviews
            .CountAsync(r => r.PlaceId == place.Id && !r.IsDeleted && r.Status == ReviewStatus.Active);

        var averageRating = 0m;
        if (reviewCount > 0)
        {
            var ratings = await reviewContext.Reviews
                .Where(r => r.PlaceId == place.Id && !r.IsDeleted && r.Status == ReviewStatus.Active)
                .Select(r => (decimal?)r.Rating)
                .ToListAsync();

            averageRating = ratings.Any() ? ratings.Average(r => r ?? 0) : 0;
        }

        // Update place rating
        if (place.AverageRating != averageRating || place.ReviewCount != reviewCount)
        {
            place.UpdateRating(averageRating, reviewCount);
            placeContext.Places.Update(place);
            updatedCount++;
            
            Console.WriteLine($"  ✓ {place.Name}: {averageRating:F1} stars ({reviewCount} reviews)");
        }
        else
        {
            skippedCount++;
            Console.WriteLine($"  - {place.Name}: Already up to date ({averageRating:F1} stars, {reviewCount} reviews)");
        }
    }

    if (updatedCount > 0)
    {
        Console.WriteLine($"\nSaving changes to database...");
        await placeContext.SaveChangesAsync();
        Console.WriteLine($"✅ Successfully updated {updatedCount} places!");
    }
    else
    {
        Console.WriteLine($"\n✅ All places are already up to date!");
    }

    if (skippedCount > 0)
    {
        Console.WriteLine($"   ({skippedCount} places were already up to date)");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"\n❌ Error: {ex.Message}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"   Inner exception: {ex.InnerException.Message}");
    }
    Console.WriteLine($"\nStack trace:\n{ex.StackTrace}");
    Environment.Exit(1);
}

Console.WriteLine("\n=== Done ===");

