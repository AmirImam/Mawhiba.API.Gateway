namespace Mawhiba.API.Gateway.Helpers;

public interface IAPIService
{
    Task<APIResult?> CallAsync(int ServiceId, string url, HttpMethod method, object? data);
}
