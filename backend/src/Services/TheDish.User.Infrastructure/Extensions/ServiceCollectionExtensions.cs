using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using TheDish.Common.Application.Interfaces;
using TheDish.Common.Infrastructure.Extensions;
using TheDish.User.Application.Interfaces;
using TheDish.User.Infrastructure.Data;
using TheDish.User.Infrastructure.Repositories;
using TheDish.User.Infrastructure.Services;

namespace TheDish.User.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<UserDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("UserDb") ?? 
                "Host=localhost;Port=5432;Database=thedish_users;Username=thedish;Password=thedish_dev_password",
                npgsqlOptions => npgsqlOptions.MigrationsAssembly("TheDish.User.Infrastructure")));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        services.AddScoped<ITokenService, TokenService>();
        services.AddHttpClient<OAuthService>();
        services.AddScoped<IOAuthService, OAuthService>();
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}

