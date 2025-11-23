using Microsoft.EntityFrameworkCore;
using Npgsql;
using TheDish.Review.Infrastructure.Data;
using TheDish.Review.Domain.Entities;

Console.WriteLine("🌱 Applying Review Service migrations...");
Console.WriteLine("==========================================\n");

var connectionString = "Host=localhost;Port=5432;Database=thedish;Username=thedish;Password=thedish_dev_password";

try
{
    Console.WriteLine("Connecting to database...");
    
    // Create a simple DbContext options without service dependencies
    var optionsBuilder = new DbContextOptionsBuilder<ReviewDbContext>();
    optionsBuilder.UseNpgsql(
        connectionString,
        npgsqlOptions => 
        {
            npgsqlOptions.UseNetTopologySuite();
            npgsqlOptions.MigrationsAssembly("TheDish.Review.Infrastructure");
        }
    );

    using var context = new ReviewDbContext(optionsBuilder.Options);
    
    // Check connection
    var canConnect = await context.Database.CanConnectAsync();
    if (!canConnect)
    {
        Console.WriteLine("❌ Cannot connect to database!");
        Environment.Exit(1);
    }
    Console.WriteLine("✅ Database connection successful!\n");
    
    // Get pending migrations
    var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
    var pendingList = pendingMigrations.ToList();
    
    if (pendingList.Count == 0)
    {
        Console.WriteLine("✅ No pending migrations. Database is up to date!");
    }
    else
    {
        Console.WriteLine($"Found {pendingList.Count} pending migration(s):");
        foreach (var migration in pendingList)
        {
            Console.WriteLine($"  - {migration}");
        }
        Console.WriteLine();
        
        Console.WriteLine("Applying migrations...");
        await context.Database.MigrateAsync();
        Console.WriteLine("✅ Review Service migrations applied successfully!");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"\n❌ Error applying migrations: {ex.Message}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
    }
    Console.WriteLine($"\nStack trace:\n{ex.StackTrace}");
    Environment.Exit(1);
}

