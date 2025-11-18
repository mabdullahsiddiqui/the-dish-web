using Microsoft.EntityFrameworkCore;
using TheDish.Place.Infrastructure.Data;
using TheDish.Place.Domain.Entities;
using TheDish.Place.Domain.Enums;
using NetTopologySuite.Geometries;
using NetTopologySuite;

Console.WriteLine("🌱 The Dish - Data Seeder");
Console.WriteLine("========================\n");

// Database connection string
var connectionString = "Host=localhost;Port=5432;Database=thedish;Username=thedish;Password=thedish_dev_password";

// Setup DbContext
var optionsBuilder = new DbContextOptionsBuilder<PlaceDbContext>();
optionsBuilder.UseNpgsql(
    connectionString,
    npgsqlOptions => npgsqlOptions.UseNetTopologySuite()
);

using var context = new PlaceDbContext(optionsBuilder.Options);

try
{
    Console.WriteLine("Connecting to database...");
    
    // Test connection
    await context.Database.CanConnectAsync();
    Console.WriteLine("✅ Database connection successful!\n");

    // Check if places already exist
    var existingCount = await context.Places.CountAsync();
    if (existingCount > 0)
    {
        Console.WriteLine($"⚠️  Found {existingCount} existing places in database.");
        Console.Write("Do you want to clear existing data and reseed? (y/N): ");
        var response = Console.ReadLine();
        
        if (response?.ToLower() == "y")
        {
            Console.WriteLine("Clearing existing places...");
            context.Places.RemoveRange(context.Places);
            await context.SaveChangesAsync();
            Console.WriteLine("✅ Existing data cleared.\n");
        }
        else
        {
            Console.WriteLine("Keeping existing data. Adding new places...\n");
        }
    }

    // Create test places
    var places = new List<Place>();

    // Joe's Pizza
    var joesPizza = new Place(
        name: "Joe's Pizza",
        address: "123 Main St, New York, NY 10001",
        latitude: 40.7128,
        longitude: -74.0060,
        priceRange: 2,
        cuisineTypes: new List<string> { "Italian", "Pizza" }
    );
    places.Add(joesPizza);

    // Green Kitchen
    var greenKitchen = new Place(
        name: "Green Kitchen",
        address: "456 Oak Ave, New York, NY 10002",
        latitude: 40.7180,
        longitude: -74.0020,
        priceRange: 3,
        cuisineTypes: new List<string> { "Vegan", "Healthy", "Organic" }
    );
    greenKitchen.UpdateDetails(phone: "(555) 234-5678", website: "https://greenkitchen.com");
    greenKitchen.UpdateDietaryTags(new Dictionary<string, bool> 
    { 
        { "Vegan", true }, 
        { "Gluten-Free", true },
        { "Organic", true }
    });
    places.Add(greenKitchen);

    // Spice Garden
    var spiceGarden = new Place(
        name: "Spice Garden",
        address: "789 Pine St, New York, NY 10003",
        latitude: 40.7150,
        longitude: -74.0080,
        priceRange: 2,
        cuisineTypes: new List<string> { "Indian", "Halal", "Spicy" }
    );
    spiceGarden.UpdateDetails(phone: "(555) 345-6789");
    spiceGarden.UpdateDietaryTags(new Dictionary<string, bool> 
    { 
        { "Halal", true },
        { "Vegetarian", true }
    });
    places.Add(spiceGarden);

    // Sushi Master
    var sushiMaster = new Place(
        name: "Sushi Master",
        address: "321 Elm St, New York, NY 10004",
        latitude: 40.7200,
        longitude: -74.0100,
        priceRange: 4,
        cuisineTypes: new List<string> { "Japanese", "Sushi", "Seafood" }
    );
    sushiMaster.UpdateDetails(phone: "(555) 456-7890", website: "https://sushimaster.com", email: "info@sushimaster.com");
    places.Add(sushiMaster);

    // Burger Palace
    var burgerPalace = new Place(
        name: "Burger Palace",
        address: "654 Maple Dr, New York, NY 10005",
        latitude: 40.7130,
        longitude: -74.0050,
        priceRange: 1,
        cuisineTypes: new List<string> { "American", "Burgers", "Fast Food" }
    );
    burgerPalace.UpdateDetails(phone: "(555) 567-8901");
    places.Add(burgerPalace);

    // Mediterranean Delight
    var mediterraneanDelight = new Place(
        name: "Mediterranean Delight",
        address: "987 Broadway, New York, NY 10006",
        latitude: 40.7140,
        longitude: -74.0070,
        priceRange: 3,
        cuisineTypes: new List<string> { "Mediterranean", "Greek", "Middle Eastern" }
    );
    mediterraneanDelight.UpdateDetails(phone: "(555) 678-9012", website: "https://mediterraneandelight.com");
    mediterraneanDelight.UpdateDietaryTags(new Dictionary<string, bool> 
    { 
        { "Halal", true }
    });
    places.Add(mediterraneanDelight);

    // Taco Fiesta
    var tacoFiesta = new Place(
        name: "Taco Fiesta",
        address: "147 5th Ave, New York, NY 10007",
        latitude: 40.7160,
        longitude: -74.0030,
        priceRange: 1,
        cuisineTypes: new List<string> { "Mexican", "Latin", "Street Food" }
    );
    tacoFiesta.UpdateDetails(phone: "(555) 789-0123");
    places.Add(tacoFiesta);

    // Thai Orchid
    var thaiOrchid = new Place(
        name: "Thai Orchid",
        address: "258 Park Ave, New York, NY 10008",
        latitude: 40.7170,
        longitude: -74.0040,
        priceRange: 2,
        cuisineTypes: new List<string> { "Thai", "Asian", "Spicy" }
    );
    thaiOrchid.UpdateDetails(phone: "(555) 890-1234", website: "https://thaiorchid.com");
    thaiOrchid.UpdateDietaryTags(new Dictionary<string, bool> 
    { 
        { "Vegetarian", true }
    });
    places.Add(thaiOrchid);

    // French Bistro
    var frenchBistro = new Place(
        name: "French Bistro",
        address: "369 Lexington Ave, New York, NY 10009",
        latitude: 40.7190,
        longitude: -74.0060,
        priceRange: 4,
        cuisineTypes: new List<string> { "French", "European", "Fine Dining" }
    );
    frenchBistro.UpdateDetails(phone: "(555) 901-2345", website: "https://frenchbistro.com", email: "reservations@frenchbistro.com");
    places.Add(frenchBistro);

    // BBQ Smokehouse
    var bbqSmokehouse = new Place(
        name: "BBQ Smokehouse",
        address: "741 Madison Ave, New York, NY 10010",
        latitude: 40.7110,
        longitude: -74.0090,
        priceRange: 2,
        cuisineTypes: new List<string> { "American", "BBQ", "Southern" }
    );
    bbqSmokehouse.UpdateDetails(phone: "(555) 012-3456");
    places.Add(bbqSmokehouse);

    Console.WriteLine($"Creating {places.Count} test places...\n");

    foreach (var place in places)
    {
        context.Places.Add(place);
    }

    await context.SaveChangesAsync();

    Console.WriteLine($"✅ Successfully seeded {places.Count} places!");
    Console.WriteLine("\nPlaces created:");
    foreach (var place in places)
    {
        Console.WriteLine($"  • {place.Name} - {place.Address}");
    }

    Console.WriteLine("\n🎉 Data seeding complete!");
}
catch (Exception ex)
{
    Console.WriteLine($"\n❌ Error seeding data: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    Environment.Exit(1);
}
