using Microsoft.Extensions.DependencyInjection;
using ShakaOrganizationPlatform.Application.Organizations.Services;

namespace ShakaOrganizationPlatform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IOrganizationService, OrganizationService>();
        return services;
    }
}
