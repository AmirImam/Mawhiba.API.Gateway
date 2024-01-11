using Mawhiba.API.Gateway.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mawhiba.API.Gateway.Helpers;

public static class ApiGatewayExtensions
{
    public static void AddGatewayServices(this IServiceCollection services)
    {
        HttpClientHandler clientHandler = new HttpClientHandler();
        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

        HttpClient http = new(clientHandler);
        services.AddScoped(sp => http);
        services.AddScoped<APIService>();
        services.AddScoped<ServiceHandler>();
        services.AddSingleton<ServiceHandlerParser>();
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    }

    public static bool IsSuccess(this ServiceInfo service, string code)
    {
        return service.SuccessCodes.Any(c => string.Equals(c, code, StringComparison.OrdinalIgnoreCase));
    }
}
