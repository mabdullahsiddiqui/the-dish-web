using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using TheDish.Place.API;
using TheDish.Place.Application.DTOs;
using TheDish.Place.Infrastructure.Data;
using Xunit;

namespace TheDish.Place.API.Tests;

public class PlacesControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public PlacesControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Replace database with in-memory for testing
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<PlaceDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<PlaceDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid().ToString());
                });
            });
        });
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreatePlace_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var createDto = new CreatePlaceDto
        {
            Name = "Test Restaurant",
            Address = "123 Main St",
            Latitude = 40.7128,
            Longitude = -74.0060,
            PriceRange = 2,
            CuisineTypes = new List<string> { "Italian" }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/places", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<Response<PlaceDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Test Restaurant");
    }

    [Fact]
    public async Task GetPlace_WithValidId_ShouldReturnOk()
    {
        // Arrange - Create a place first
        var createDto = new CreatePlaceDto
        {
            Name = "Test Restaurant",
            Address = "123 Main St",
            Latitude = 40.7128,
            Longitude = -74.0060,
            PriceRange = 2
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/places", createDto);
        var createdPlace = await createResponse.Content.ReadFromJsonAsync<Response<PlaceDto>>();
        var placeId = createdPlace!.Data!.Id;

        // Act
        var response = await _client.GetAsync($"/api/v1/places/{placeId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Response<PlaceDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data!.Id.Should().Be(placeId);
    }

    [Fact]
    public async Task GetNearbyPlaces_WithValidCoordinates_ShouldReturnOk()
    {
        // Arrange - Create places first
        var place1 = new CreatePlaceDto
        {
            Name = "Restaurant 1",
            Address = "123 Main St",
            Latitude = 40.7128,
            Longitude = -74.0060,
            PriceRange = 2
        };

        await _client.PostAsJsonAsync("/api/v1/places", place1);

        // Act
        var response = await _client.GetAsync("/api/v1/places/nearby?latitude=40.7128&longitude=-74.0060&radiusKm=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Response<List<PlaceDto>>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }
}

