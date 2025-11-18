using TheDish.Review.Infrastructure.Extensions;
using TheDish.Review.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using TheDish.Common.Infrastructure.Extensions;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Review Infrastructure
builder.Services.AddReviewInfrastructure(builder.Configuration);

// Add Common Infrastructure (MediatR, FluentValidation)
var applicationAssembly = typeof(TheDish.Review.Application.Commands.CreateReviewCommand).Assembly;
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
    var context = scope.ServiceProvider.GetRequiredService<ReviewDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
