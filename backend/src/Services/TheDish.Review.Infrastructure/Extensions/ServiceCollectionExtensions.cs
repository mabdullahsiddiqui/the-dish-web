using Amazon.S3;
using Amazon.Extensions.NETCore.Setup;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheDish.Review.Application.Interfaces;
using TheDish.Review.Infrastructure.Data;
using TheDish.Review.Infrastructure.Repositories;
using TheDish.Review.Infrastructure.Services;

namespace TheDish.Review.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddReviewInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ReviewDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("ReviewDb"),
                npgsqlOptions => npgsqlOptions.UseNetTopologySuite()));

        // Repositories
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        services.AddScoped<IGpsVerificationService, GpsVerificationService>();
        services.AddScoped<IPhotoService, PhotoService>();

        // HTTP Client for inter-service communication
        services.AddHttpClient();

        // AWS S3
        var awsOptions = configuration.GetAWSOptions();
        services.AddDefaultAWSOptions(awsOptions);
        services.AddAWSService<IAmazonS3>();

        return services;
    }
}

