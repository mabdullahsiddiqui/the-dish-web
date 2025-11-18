using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TheDish.Common.Application.Interfaces;
using TheDish.Common.Application.Behaviors;
using MediatR;
using System.Reflection;

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

        return services;
    }
}

