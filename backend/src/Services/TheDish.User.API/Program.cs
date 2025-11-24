using TheDish.User.Infrastructure.Extensions;
using TheDish.User.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using TheDish.Common.Infrastructure.Extensions;
using System.Reflection;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add User Infrastructure
builder.Services.AddUserInfrastructure(builder.Configuration);

// Add Common Infrastructure (MediatR, FluentValidation)
var applicationAssembly = typeof(TheDish.User.Application.Commands.RegisterUserCommand).Assembly;
builder.Services.AddCommonInfrastructure(builder.Configuration, applicationAssembly);

// Add Health Checks
var userDbConnectionString = builder.Configuration.GetConnectionString("UserDb") ?? 
    "Host=localhost;Port=5432;Database=thedish_users;Username=thedish;Password=thedish_dev_password";
builder.Services.AddHealthChecks()
    .AddNpgSql(userDbConnectionString, name: "postgresql");

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapHealthChecks("/health");

// Apply database migrations
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    // Apply migrations to ensure database schema is up to date
    try
    {
        context.Database.Migrate();
        logger.LogInformation("Database migrations applied successfully.");
    }
    catch (Exception ex)
    {
        // If migrations fail, log and try EnsureCreated as fallback
        logger.LogWarning(ex, "Failed to apply migrations. Attempting EnsureCreated as fallback.");
        try
        {
            context.Database.EnsureCreated();
            logger.LogInformation("Database created using EnsureCreated.");
        }
        catch (Exception ex2)
        {
            logger.LogError(ex2, "Failed to create database. Please check database connection and permissions.");
        }
    }
}

app.Run();
