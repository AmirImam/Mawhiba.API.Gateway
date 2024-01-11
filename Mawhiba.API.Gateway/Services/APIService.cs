using ApiGatewayService.API.ServiceHandlers;
using System.Text;

namespace Mawhiba.API.Gateway.Services;

public class APIService //: IAPIService
{
    private HttpClient Http
    {
        get
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            HttpClient http = new(clientHandler, false);
            return http;
        }
    }
    private readonly ServiceHandlerParser serviceHandlerParser;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IWebHostEnvironment webHostEnvironment;
    private ServiceHandler _serviceHandler;

    public APIService(ServiceHandlerParser serviceHandlerParser,
        IHttpContextAccessor httpContextAccessor,
        IWebHostEnvironment webHostEnvironment)
    {

        this.serviceHandlerParser = serviceHandlerParser;
        this.httpContextAccessor = httpContextAccessor;
        this.webHostEnvironment = webHostEnvironment;
        //_serviceHandler = serviceHandler;
    }

    public virtual async Task<APIResult?> CallAsync(int serviceId, string url, HttpMethod method, object data)
    {
        StringContent? stringContent = null;
        if (data != null)
        {
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            stringContent = content;
        }

        return await ExecuteCallAsync(serviceId, url, method, stringContent);

    }
    public virtual async Task<APIResult?> CallAsync(int serviceId, string url, HttpMethod method, StringContent? data)
    {
        return await ExecuteCallAsync(serviceId, url, method, data);
    }

    private async Task<APIResult?> ExecuteCallAsync(int serviceId, string url, HttpMethod method, StringContent? data)
    {
        try
        {
            _serviceHandler = serviceHandlerParser.GetServiceByServiceId(_serviceHandler, serviceId, webHostEnvironment);
            
            string fullUrl = $"{_serviceHandler.CurrentServiceInfo.BaseUrl.TrimEnd('/')}/{url}";

            HttpRequestMessage requestMessage = new()
            {
                Method = method,
                RequestUri = new Uri(fullUrl)
            };

            var request = _serviceHandler.HandleRequest(requestMessage, httpContextAccessor.HttpContext.Request);
            if (data != null)
            {
                request.Content = data;
            }

            using (var _http = Http)
            {
                _http.BaseAddress = new Uri(_serviceHandler.CurrentServiceInfo.BaseUrl);
                var response = await _http.SendAsync(request);
                return await _serviceHandler.HandleResponseAsync(response);
            }
        }
        catch (Exception ex)
        {
            return _serviceHandler.HandleException(ex);
        }
    }
}

