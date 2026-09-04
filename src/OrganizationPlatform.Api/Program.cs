using organizationPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using organizationPlatform.Application.Interface;
using organizationPlatform.Infrastructure.Services;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddDbContext<AppDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("AppDatabase"))


.LogTo(Console.WriteLine, LogLevel.Information) // Log SQL to output window
.EnableSensitiveDataLogging()); // Show parameters in querylogs (dev only)
builder.Services.AddScoped<IBudgetsService, BudgetService>();


builder.Services.AddControllers();
// OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();


// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();
