using ApiGatewayService.API.ServiceHandlers;
using System.Text;

namespace Mawhiba.API.Gateway.Services;

public class APIService : IAPIService
{
    private readonly HttpClient _httpClient;
    private readonly ServiceHandlerParser serviceHandlerParser;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IWebHostEnvironment webHostEnvironment;
    private ServiceHandler _serviceHandler;

    public APIService(HttpClient httpClient, ServiceHandlerParser serviceHandlerParser,
        IHttpContextAccessor httpContextAccessor,
        IWebHostEnvironment webHostEnvironment)
    {
        _httpClient = httpClient;
        this.serviceHandlerParser = serviceHandlerParser;
        this.httpContextAccessor = httpContextAccessor;
        this.webHostEnvironment = webHostEnvironment;
        //_serviceHandler = serviceHandler;
    }

    public virtual async Task<APIResult?> CallAsync(int serviceId, string url, HttpMethod method, object data)
    {
        try
        {
            _serviceHandler = serviceHandlerParser.GetServiceByServiceId(_serviceHandler, serviceId);
            var requestMessage = new HttpRequestMessage(method, url);
            var request = _serviceHandler.HandleRequest(requestMessage,httpContextAccessor.HttpContext.Request);

            if (data != null)
            {
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                request.Content = content;
            }
            var serviceInfo = ReadServiceJsonFile(serviceId);
            _httpClient.BaseAddress = new Uri(serviceInfo.BaseUrl);
            var response = await _httpClient.SendAsync(request);
            return await _serviceHandler.HandleResponseAsync(response);
        }
        catch (Exception ex)
        {
            return _serviceHandler.HandleException(ex);
        }
    }

    private ServiceInfo ReadServiceJsonFile(int serviceId)
    {
        var _hostingEnvironment = webHostEnvironment;// this.ServiceProvider.GetService<IHttpContextAccessor>();
        string file = Path.Combine(_hostingEnvironment.ContentRootPath, "wwwroot\\Services", $"{serviceId}.json");
        StreamReader reader = new(file);
        string content = reader.ReadToEnd();
        reader.Close();
        ServiceInfo info = Newtonsoft.Json.JsonConvert.DeserializeObject<ServiceInfo>(content);
        return info;
        //jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string,string>>(content);
    }
}

