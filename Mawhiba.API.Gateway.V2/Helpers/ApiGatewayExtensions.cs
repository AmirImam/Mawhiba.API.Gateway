using Mawhiba.API.Gateway.V2.Controllers;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mawhiba.API.Gateway.V2.Helpers;

public static class ApiGatewayExtensions
{
    public static void AddGatewayServices(this IServiceCollection services)
    {
        services.AddScoped<HttpClient>();
        services.AddScoped<ServiceHandler>();
        
        services.AddScoped<BebrasServiceHandler>();
        services.AddScoped<KangarooServiceHandler>();
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<ServicesCaller>();
        services.AddScoped<ServiceInfo>();
    }
}
