﻿namespace Mawhiba.API.Gateway.Helpers;

public class ServiceInfo
{
    public string BaseUrl { get; set; } = string.Empty;
    public List<Endpoint>? Endpoints { get; set; }
    public IEnumerable<string> SuccessCodes { get; set; } = new List<string>();
    public IEnumerable<string> FailedCodes { get; set; } = new List<string>();
}

public class Endpoint
{
    public string Key { get; set; }
    public string Route { get; set; }
    public bool Authorized { get; set; }
}
