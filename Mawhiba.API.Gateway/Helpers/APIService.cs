using ApiGatewayService.API.ServiceHandlers;
using System.Text;

namespace Mawhiba.API.Gateway.Helpers;

public class APIService : IAPIService
{
    private readonly HttpClient _httpClient;
    private readonly ServiceHandlerParser serviceHandlerParser;
    private ServiceHandler _serviceHandler;

    public APIService(HttpClient httpClient,ServiceHandlerParser serviceHandlerParser)
    {
        _httpClient = httpClient;
        this.serviceHandlerParser = serviceHandlerParser;
        //_serviceHandler = serviceHandler;
    }

    public virtual async Task<APIResult?> CallAsync(int ServiceId, string url, HttpMethod method, object data)
    {
        try
        {
            _serviceHandler = serviceHandlerParser.GetServiceByServiceId(_serviceHandler, ServiceId);
            var requestMessage = new HttpRequestMessage(method, url);
            var request = _serviceHandler.HandleRequest(requestMessage);

            if (data != null)
            {
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                request.Content = content;
            }

            var response = await _httpClient.SendAsync(request);
            return await _serviceHandler.HandleResponseAsync(response);
        }
        catch (Exception ex)
        {
            return _serviceHandler.HandleException(ex);
        }
    }

   
}

