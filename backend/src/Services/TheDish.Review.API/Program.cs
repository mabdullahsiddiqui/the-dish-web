using TheDish.Review.Infrastructure.Extensions;
using TheDish.Review.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using TheDish.Common.Infrastructure.Extensions;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["Key"] ?? jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT Key not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Add Review Infrastructure
builder.Services.AddReviewInfrastructure(builder.Configuration);

// Add Common Infrastructure (MediatR, FluentValidation)
var applicationAssembly = typeof(TheDish.Review.Application.Commands.CreateReviewCommand).Assembly;
builder.Services.AddCommonInfrastructure(builder.Configuration, applicationAssembly);

// Add Health Checks
var reviewDbConnectionString = builder.Configuration.GetConnectionString("ReviewDb") ?? 
    "Host=localhost;Port=5432;Database=thedish;Username=thedish;Password=thedish_dev_password";
builder.Services.AddHealthChecks()
    .AddNpgSql(reviewDbConnectionString, name: "postgresql");

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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapHealthChecks("/health");

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ReviewDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
