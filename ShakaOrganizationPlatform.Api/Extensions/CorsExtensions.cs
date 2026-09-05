namespace ShakaOrganizationPlatform.Api.Extensions;

public static class CorsExtensions
{
    public const string AngularPolicy = "AngularApp";

    public static IServiceCollection AddAngularCors(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(AngularPolicy, policy =>
            {
                policy.SetIsOriginAllowed(_ => true)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }
}
