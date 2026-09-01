using Microsoft.Extensions.DependencyInjection;

namespace ShakaOrganizationPlatform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register application services, handlers, validators, etc.
        return services;
    }
}

