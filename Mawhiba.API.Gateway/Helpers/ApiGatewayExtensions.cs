namespace Mawhiba.API.Gateway.Helpers;

public static class ApiGatewayExtensions
{
    public static void AddGatewayServices(this IServiceCollection services)
    {
        services.AddScoped<HttpClient>();
        services.AddScoped<IAPIService, APIService>();
        services.AddScoped<ServiceHandler>();
        services.AddSingleton<ServiceHandlerParser>();
    }
}
