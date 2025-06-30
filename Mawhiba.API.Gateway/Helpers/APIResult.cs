namespace Mawhiba.API.Gateway.Helpers;

public class APIResult
{
    public string ResultCode { get; set; } = string.Empty;
    public string ResultMessage { get; set; } = string.Empty;
    public string MoreDetails { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
    public Dictionary<string, string>? ValidationDictionary { get; set; }
    public object? ResultObject { get; set; }
    //public object? resultObject { get; set; }
}

public class APIResult2
{
    public string ResultCode { get; set; } = string.Empty;
    public string ResultMessage { get; set; } = string.Empty;
    public string MoreDetails { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
    public Dictionary<string, string>? ValidationDictionary { get; set; }
    public object? resultObject { get; set; }
}
