using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TheDish.Common.Application.Interfaces;
using TheDish.Common.Application.Behaviors;
using MediatR;
using System.Reflection;
using Amazon.S3;
using Microsoft.Extensions.Options;
using TheDish.Common.Infrastructure.Configuration;
using TheDish.Common.Infrastructure.Services;

namespace TheDish.Common.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommonInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        Assembly applicationAssembly)
    {
        // MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(applicationAssembly);
        });
        
        // Add pipeline behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        // AWS S3
        services.Configure<AwsS3Settings>(configuration.GetSection("AWS:S3"));
        services.AddDefaultAWSOptions(configuration.GetAWSOptions());
        services.AddAWSService<IAmazonS3>();
        services.AddScoped<IFileStorageService, AwsS3Service>();

        return services;
    }
}

