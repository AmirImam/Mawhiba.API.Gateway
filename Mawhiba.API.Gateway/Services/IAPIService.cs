namespace Mawhiba.API.Gateway.Services;

public interface IAPIService
{
    Task<APIResult?> CallAsync(int ServiceId, string url, HttpMethod method, object? data);
}
