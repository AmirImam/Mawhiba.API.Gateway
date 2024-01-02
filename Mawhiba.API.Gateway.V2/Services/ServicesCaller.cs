namespace Mawhiba.API.Gateway.V2.Services;

public class ServicesCaller
{
    public ServicesCaller(HttpClient http, 
        ServiceHandler serviceHandler, 
        IHttpContextAccessor httpContextAccessor, 
        IWebHostEnvironment webHostEnvironment)
    {
        Http = http;
        ServiceHandler = serviceHandler;
        HttpContextAccessor = httpContextAccessor;
        WebHostEnvironment = webHostEnvironment;
        
    }

    public HttpClient Http { get; }
    public ServiceHandler ServiceHandler { get; }
    public IHttpContextAccessor HttpContextAccessor { get; }
    public IWebHostEnvironment WebHostEnvironment { get; }
    public ServiceInfo ServiceInfo { get; set; }
}