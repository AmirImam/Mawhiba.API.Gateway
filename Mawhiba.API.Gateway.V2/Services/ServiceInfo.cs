namespace Mawhiba.API.Gateway.V2.Services;

public class ServiceInfo
{
    public string BaseUrl { get; set; } = string.Empty;
    public List<Endpoint>? Endpoints { get; set; }
}

public class Endpoint
{
    public string Key { get; set; }
    public string Route { get; set; }
    public bool Authorized { get; set; }
}
