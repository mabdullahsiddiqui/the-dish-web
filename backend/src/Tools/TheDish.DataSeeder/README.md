# The Dish - Data Seeder

A console application to seed test data into The Dish database.

## Prerequisites

1. Docker containers must be running (PostgreSQL)
2. Database migrations must be applied
3. .NET 8 SDK installed

## Usage

### Option 1: Use the PowerShell Script (Recommended)

```powershell
# From backend directory
.\scripts\seed-data.ps1
```

### Option 2: Run Directly

```powershell
# From backend/src/Tools/TheDish.DataSeeder directory
dotnet run
```

## What It Seeds

The seeder creates **10 test restaurants** with:
- Various cuisines (Italian, Vegan, Indian, Japanese, etc.)
- Different price ranges ($ to $$$$)
- Dietary tags (Halal, Vegan, Gluten-Free, etc.)
- Contact information (phone, website, email)
- Locations in New York area

## Test Places Created

1. **Joe's Pizza** - Italian, Pizza ($)
2. **Green Kitchen** - Vegan, Healthy, Organic ($$$)
3. **Spice Garden** - Indian, Halal, Spicy ($$)
4. **Sushi Master** - Japanese, Sushi, Seafood ($$$$)
5. **Burger Palace** - American, Burgers, Fast Food ($)
6. **Mediterranean Delight** - Mediterranean, Greek, Middle Eastern ($$$)
7. **Taco Fiesta** - Mexican, Latin, Street Food ($)
8. **Thai Orchid** - Thai, Asian, Spicy ($$)
9. **French Bistro** - French, European, Fine Dining ($$$$)
10. **BBQ Smokehouse** - American, BBQ, Southern ($$)

## Database Connection

The seeder uses the default connection string:
```
Host=localhost;Port=5432;Database=thedish;Username=thedish;Password=thedish_dev_password
```

To change this, edit `Program.cs` and update the `connectionString` variable.

## Notes

- If places already exist, the script will ask if you want to clear them
- All places are created with `Active` status
- Locations are set in the New York area (latitude ~40.71, longitude ~-74.00)




