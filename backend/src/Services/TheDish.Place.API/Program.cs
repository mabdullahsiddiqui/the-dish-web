using TheDish.Place.Infrastructure.Extensions;
using TheDish.Place.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using TheDish.Common.Infrastructure.Extensions;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Place Infrastructure
builder.Services.AddPlaceInfrastructure(builder.Configuration);

// Add Common Infrastructure (MediatR, FluentValidation)
var applicationAssembly = typeof(TheDish.Place.Application.Commands.CreatePlaceCommand).Assembly;
builder.Services.AddCommonInfrastructure(builder.Configuration, applicationAssembly);

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

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PlaceDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
