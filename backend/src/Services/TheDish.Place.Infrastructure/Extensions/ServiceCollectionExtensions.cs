using Amazon.S3;
using Amazon.Extensions.NETCore.Setup;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheDish.Place.Application.Interfaces;
using TheDish.Place.Infrastructure.Data;
using TheDish.Place.Infrastructure.Repositories;
using TheDish.Place.Infrastructure.Services;

namespace TheDish.Place.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPlaceInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<PlaceDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("PlaceDb"),
                npgsqlOptions => npgsqlOptions.UseNetTopologySuite()));

        // Repositories
        services.AddScoped<IPlaceRepository, PlaceRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        services.AddScoped<IPhotoService, PhotoService>();

        // AWS S3
        var awsOptions = configuration.GetAWSOptions();
        services.AddDefaultAWSOptions(awsOptions);
        services.AddAWSService<IAmazonS3>();

        return services;
    }
}

