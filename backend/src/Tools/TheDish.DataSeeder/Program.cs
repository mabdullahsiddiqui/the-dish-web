using Microsoft.EntityFrameworkCore;
using TheDish.Place.Infrastructure.Data;
using TheDish.Place.Domain.Entities;
using TheDish.Place.Domain.Enums;
using TheDish.User.Infrastructure.Data;
using TheDish.User.Domain.Entities;
using TheDish.Review.Infrastructure.Data;
using TheDish.Review.Domain.Entities;
using NetTopologySuite.Geometries;
using NetTopologySuite;

Console.WriteLine("🌱 The Dish - Data Seeder");
Console.WriteLine("========================\n");

// Database connection strings (each service uses its own database)
var placeConnectionString = "Host=localhost;Port=5432;Database=thedish;Username=thedish;Password=thedish_dev_password";
var userConnectionString = "Host=localhost;Port=5432;Database=thedish_users;Username=thedish;Password=thedish_dev_password";
var reviewConnectionString = "Host=localhost;Port=5432;Database=thedish;Username=thedish;Password=thedish_dev_password";

// Setup DbContexts
var placeOptionsBuilder = new DbContextOptionsBuilder<PlaceDbContext>();
placeOptionsBuilder.UseNpgsql(
    placeConnectionString,
    npgsqlOptions => npgsqlOptions.UseNetTopologySuite()
);

var userOptionsBuilder = new DbContextOptionsBuilder<UserDbContext>();
userOptionsBuilder.UseNpgsql(userConnectionString);

var reviewOptionsBuilder = new DbContextOptionsBuilder<ReviewDbContext>();
reviewOptionsBuilder.UseNpgsql(
    reviewConnectionString,
    npgsqlOptions => npgsqlOptions.UseNetTopologySuite()
);

using var placeContext = new PlaceDbContext(placeOptionsBuilder.Options);
using var userContext = new UserDbContext(userOptionsBuilder.Options);
using var reviewContext = new ReviewDbContext(reviewOptionsBuilder.Options);

try
{
    Console.WriteLine("Connecting to database...");
    
    // Test connections
    await placeContext.Database.CanConnectAsync();
    await userContext.Database.CanConnectAsync();
    await reviewContext.Database.CanConnectAsync();
    Console.WriteLine("✅ Database connection successful!\n");

    // Check if places already exist
    var existingPlaceCount = await placeContext.Places.CountAsync();
    if (existingPlaceCount == 0)
    {
        Console.WriteLine("⚠️  No places found. Creating places first...\n");
        
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
            placeContext.Places.Add(place);
        }

        await placeContext.SaveChangesAsync();
        Console.WriteLine($"✅ Successfully seeded {places.Count} places!\n");
    }
    else
    {
        Console.WriteLine($"✅ Found {existingPlaceCount} existing places.\n");
    }

    // Get all places from database
    var allPlaces = await placeContext.Places.ToListAsync();
    Console.WriteLine($"Working with {allPlaces.Count} places.\n");

    // Check if users already exist
    var existingUserCount = await userContext.Users.CountAsync();
    List<User> users;

    // We need at least 20 users to create 10-20 reviews per place
    // If we have fewer than 20 users, create more
    if (existingUserCount < 20)
    {
        Console.WriteLine($"Found {existingUserCount} existing users. Creating additional users to reach 20...\n");
        
        // Get existing emails to avoid duplicates
        var existingEmails = await userContext.Users.Select(u => u.Email).ToListAsync();
        
        // Create 20 test users
        var userNames = new[]
        {
            ("John", "Smith", "john.smith@example.com"),
            ("Sarah", "Johnson", "sarah.johnson@example.com"),
            ("Michael", "Williams", "michael.williams@example.com"),
            ("Emily", "Brown", "emily.brown@example.com"),
            ("David", "Jones", "david.jones@example.com"),
            ("Jessica", "Garcia", "jessica.garcia@example.com"),
            ("James", "Miller", "james.miller@example.com"),
            ("Ashley", "Davis", "ashley.davis@example.com"),
            ("Robert", "Rodriguez", "robert.rodriguez@example.com"),
            ("Amanda", "Martinez", "amanda.martinez@example.com"),
            ("William", "Hernandez", "william.hernandez@example.com"),
            ("Melissa", "Lopez", "melissa.lopez@example.com"),
            ("Richard", "Wilson", "richard.wilson@example.com"),
            ("Nicole", "Anderson", "nicole.anderson@example.com"),
            ("Joseph", "Thomas", "joseph.thomas@example.com"),
            ("Michelle", "Taylor", "michelle.taylor@example.com"),
            ("Thomas", "Moore", "thomas.moore@example.com"),
            ("Laura", "Jackson", "laura.jackson@example.com"),
            ("Charles", "Lee", "charles.lee@example.com"),
            ("Kimberly", "White", "kimberly.white@example.com")
        };

        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Test123!");
        var newUsers = new List<User>();

        foreach (var (firstName, lastName, email) in userNames)
        {
            if (!existingEmails.Contains(email))
            {
                var user = User.CreateWithEmail(email, firstName, lastName, passwordHash);
                newUsers.Add(user);
                userContext.Users.Add(user);
            }
        }

        if (newUsers.Count > 0)
        {
            await userContext.SaveChangesAsync();
            Console.WriteLine($"✅ Created {newUsers.Count} new users!\n");
        }

        // Get all users (existing + newly created)
        users = await userContext.Users.ToListAsync();
        Console.WriteLine($"✅ Total users available: {users.Count}\n");
    }
    else
    {
        users = await userContext.Users.ToListAsync();
        Console.WriteLine($"✅ Found {users.Count} existing users.\n");
    }

    // Ensure we have enough users (at least 20)
    if (users.Count < 20)
    {
        Console.WriteLine($"⚠️  Warning: Only {users.Count} users available. Some places may have fewer reviews.\n");
    }

    // Check if reviews already exist
    var existingReviewCount = await reviewContext.Reviews.CountAsync();
    if (existingReviewCount > 0)
    {
        Console.WriteLine($"⚠️  Found {existingReviewCount} existing reviews.");
        Console.Write("Do you want to clear existing reviews and reseed? (y/N): ");
        var response = Console.ReadLine();
        
        if (response?.ToLower() == "y")
        {
            Console.WriteLine("Clearing existing reviews...");
            reviewContext.Reviews.RemoveRange(reviewContext.Reviews);
            await reviewContext.SaveChangesAsync();
            Console.WriteLine("✅ Existing reviews cleared.\n");
        }
        else
        {
            Console.WriteLine("Keeping existing reviews. Exiting...\n");
            return;
        }
    }

    // Review text templates
    var reviewTexts = new[]
    {
        "Great food and service! Will definitely come back.",
        "The atmosphere was nice but the food was just okay.",
        "Amazing experience! The staff was friendly and the food was delicious.",
        "Not worth the price. Food was cold and service was slow.",
        "Excellent quality and presentation. Highly recommend!",
        "Good value for money. The portions were generous.",
        "The ambiance is perfect for a date night. Food was decent.",
        "Overpriced for what you get. Service needs improvement.",
        "Best [cuisine] in the area! Authentic flavors and great service.",
        "The [dish] was outstanding, but the rest was average.",
        "Clean restaurant with friendly staff. Food came out quickly.",
        "Disappointing experience. Food was bland and overpriced.",
        "Love this place! Been coming here for years. Always consistent.",
        "The decor is beautiful but the food didn't match the atmosphere.",
        "Great for families. Kids loved it and the staff was accommodating.",
        "Perfect spot for lunch. Quick service and fresh ingredients.",
        "The chef clearly knows what they're doing. Every dish was perfect.",
        "Not impressed. Expected more based on the reviews.",
        "Hidden gem! Found this place by accident and so glad I did.",
        "The [dietary] options were clearly marked and tasted great!",
        "Would give it 5 stars if the service was faster.",
        "Great location, decent food, reasonable prices. Worth a visit.",
        "The menu has something for everyone. Great variety!",
        "Service was excellent but the food was just average.",
        "One of my favorite places in the city. Never disappoints!",
        "The presentation was beautiful but flavors were lacking.",
        "Perfect for a quick bite. Fast, affordable, and tasty.",
        "The staff went above and beyond. Made our evening special.",
        "Good food but the wait time was too long.",
        "Authentic [cuisine] experience. Felt like I was in [country]!"
    };

    var random = new Random();
    var allReviews = new List<Review>();
    var reviewCount = 0;

    Console.WriteLine("Creating reviews for each place...\n");

    foreach (var place in allPlaces)
    {
        // Get place location
        var placeLat = place.Location.Y;
        var placeLon = place.Location.X;
        
        // Get dietary tags for this place
        var dietaryTags = place.DietaryTags.Keys.ToList();
        
        // Create 10-20 reviews for this place (but not more than available users)
        var maxReviewsForPlace = Math.Min(random.Next(10, 21), users.Count);
        var usedUserIds = new HashSet<Guid>();
        
        for (int i = 0; i < maxReviewsForPlace; i++)
        {
            // Select a random user that hasn't reviewed this place yet
            var availableUsers = users.Where(u => !usedUserIds.Contains(u.Id)).ToList();
            if (availableUsers.Count == 0)
            {
                // No more available users for this place
                break;
            }
            
            var user = availableUsers[random.Next(availableUsers.Count)];
            usedUserIds.Add(user.Id);

            // Rating distribution: 20% 5-star, 30% 4-star, 30% 3-star, 15% 2-star, 5% 1-star
            var ratingRoll = random.Next(100);
            int rating;
            if (ratingRoll < 20) rating = 5;
            else if (ratingRoll < 50) rating = 4;
            else if (ratingRoll < 80) rating = 3;
            else if (ratingRoll < 95) rating = 2;
            else rating = 1;

            // Select review text
            var reviewText = reviewTexts[random.Next(reviewTexts.Length)]
                .Replace("[cuisine]", place.CuisineTypes.FirstOrDefault() ?? "food")
                .Replace("[dish]", GetRandomDish(place.CuisineTypes))
                .Replace("[dietary]", dietaryTags.FirstOrDefault() ?? "dietary")
                .Replace("[country]", GetCountryForCuisine(place.CuisineTypes));

            // Create review
            var review = new Review(user.Id, place.Id, rating, reviewText);

            // GPS verification (70% verified)
            var isGpsVerified = random.Next(100) < 70;
            if (isGpsVerified)
            {
                // Check-in location near the place (within 0.01 degrees, roughly 1km)
                var offsetLat = (random.NextDouble() - 0.5) * 0.01;
                var offsetLon = (random.NextDouble() - 0.5) * 0.01;
                review.SetGpsVerification(true, placeLat + offsetLat, placeLon + offsetLon);
            }

            // Dietary accuracy (only for places with dietary tags)
            if (dietaryTags.Count > 0 && random.Next(100) < 60) // 60% of reviews include dietary accuracy
            {
                var dietaryAccuracy = new Dictionary<string, string>();
                foreach (var tag in dietaryTags)
                {
                    var accuracyRoll = random.Next(100);
                    if (accuracyRoll < 70) dietaryAccuracy[tag] = "accurate";
                    else if (accuracyRoll < 85) dietaryAccuracy[tag] = "inaccurate";
                    else dietaryAccuracy[tag] = "unsure";
                }
                review.SetDietaryAccuracy(dietaryAccuracy);
            }

            // Helpfulness counts (some reviews have votes)
            if (random.Next(100) < 70) // 70% of reviews have some helpfulness votes
            {
                var helpfulCount = random.Next(0, 51);
                var notHelpfulCount = random.Next(0, 11);
                
                for (int h = 0; h < helpfulCount; h++)
                {
                    review.IncrementHelpfulCount();
                }
                for (int n = 0; n < notHelpfulCount; n++)
                {
                    review.IncrementNotHelpfulCount();
                }
            }

            allReviews.Add(review);
            reviewCount++;
        }

        Console.WriteLine($"  • {place.Name}: {maxReviewsForPlace} reviews");
    }

    // Save reviews in batches to avoid issues
    Console.WriteLine("\nSaving reviews to database...");
    const int batchSize = 50;
    for (int i = 0; i < allReviews.Count; i += batchSize)
    {
        var batch = allReviews.Skip(i).Take(batchSize).ToList();
        foreach (var review in batch)
        {
            reviewContext.Reviews.Add(review);
        }
        try
        {
            await reviewContext.SaveChangesAsync();
            Console.WriteLine($"  Saved batch {i / batchSize + 1} ({Math.Min(i + batchSize, allReviews.Count)}/{allReviews.Count} reviews)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ Error saving batch {i / batchSize + 1}: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            throw;
        }
    }

    // Update user review counts
    Console.WriteLine("\nUpdating user review counts...");
    foreach (var user in users)
    {
        var userReviewCount = allReviews.Count(r => r.UserId == user.Id);
        for (int i = 0; i < userReviewCount; i++)
        {
            user.IncrementReviewCount();
        }
    }
    await userContext.SaveChangesAsync();

    Console.WriteLine($"\n✅ Successfully seeded {reviewCount} reviews!");
    Console.WriteLine($"✅ Updated review counts for {users.Count} users!");
    Console.WriteLine("\n🎉 Data seeding complete!");
}
catch (Exception ex)
{
    Console.WriteLine($"\n❌ Error seeding data: {ex.Message}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
    }
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    Environment.Exit(1);
}

// Helper methods
string GetRandomDish(List<string> cuisineTypes)
{
    var dishes = new Dictionary<string, string[]>
    {
        { "Italian", new[] { "pasta", "pizza", "risotto", "lasagna" } },
        { "Pizza", new[] { "margherita", "pepperoni", "quattro formaggi" } },
        { "Vegan", new[] { "quinoa bowl", "veggie burger", "smoothie bowl" } },
        { "Indian", new[] { "butter chicken", "biryani", "tikka masala" } },
        { "Japanese", new[] { "sushi roll", "ramen", "teriyaki" } },
        { "Sushi", new[] { "salmon roll", "tuna sashimi", "dragon roll" } },
        { "American", new[] { "burger", "fries", "steak" } },
        { "Burgers", new[] { "classic burger", "cheeseburger", "bacon burger" } },
        { "Mediterranean", new[] { "hummus", "falafel", "shawarma" } },
        { "Mexican", new[] { "tacos", "burrito", "quesadilla" } },
        { "Thai", new[] { "pad thai", "green curry", "tom yum" } },
        { "French", new[] { "coq au vin", "ratatouille", "bouillabaisse" } },
        { "BBQ", new[] { "brisket", "ribs", "pulled pork" } }
    };

    foreach (var cuisine in cuisineTypes)
    {
        if (dishes.ContainsKey(cuisine))
        {
            return dishes[cuisine][new Random().Next(dishes[cuisine].Length)];
        }
    }
    return "signature dish";
}

string GetCountryForCuisine(List<string> cuisineTypes)
{
    var countries = new Dictionary<string, string>
    {
        { "Italian", "Italy" },
        { "Indian", "India" },
        { "Japanese", "Japan" },
        { "Thai", "Thailand" },
        { "French", "France" },
        { "Mexican", "Mexico" },
        { "Mediterranean", "Greece" },
        { "Greek", "Greece" }
    };

    foreach (var cuisine in cuisineTypes)
    {
        if (countries.ContainsKey(cuisine))
        {
            return countries[cuisine];
        }
    }
    return "the region";
}
