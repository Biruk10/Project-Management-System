using ShakaOrganizationPlatform.Api.Extensions;
using ShakaOrganizationPlatform.Api.Middleware;
using ShakaOrganizationPlatform.Application;
using ShakaOrganizationPlatform.Infrastructure;
using ShakaOrganizationPlatform.Infrastructure.Persistence;
using ShakaOrganizationPlatform.Infrastructure.Persistence.Seed;

using System.Text.Json.Serialization;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddSwaggerWithJwt();
builder.Services.AddAngularCors(builder.Configuration);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
    await PermissionSeeder.SeedAsync(db);
    await SystemAdminSeeder.SeedAsync(db);
    await RolePermissionSeeder.RepairExistingOrganizationsAsync(db);
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors(CorsExtensions.AngularPolicy);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrganizationPlatform API V1");
        c.RoutePrefix = "swagger";
    });
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors(CorsExtensions.AngularPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
