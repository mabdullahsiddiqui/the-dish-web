using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheDish.AI.ReviewAnalysis.Application.Interfaces;
using TheDish.AI.ReviewAnalysis.Infrastructure.Consumers;
using TheDish.AI.ReviewAnalysis.Infrastructure.Services;

namespace TheDish.AI.ReviewAnalysis.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAIReviewAnalysisInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Services
        services.AddSingleton<ISentimentAnalysisService, SentimentAnalysisService>();
        services.AddScoped<IElasticsearchService, ElasticsearchService>();

        // MassTransit RabbitMQ
        services.AddMassTransit(x =>
        {
            x.AddConsumer<ReviewCreatedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["RabbitMQ:Host"] ?? "localhost", "/", h =>
                {
                    h.Username(configuration["RabbitMQ:Username"] ?? "thedish");
                    h.Password(configuration["RabbitMQ:Password"] ?? "thedish_dev_password");
                });

                cfg.ReceiveEndpoint("review.created", e =>
                {
                    e.ConfigureConsumer<ReviewCreatedConsumer>(context);
                    e.PrefetchCount = 10;
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}






