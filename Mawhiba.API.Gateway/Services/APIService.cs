using ApiGatewayService.API.ServiceHandlers;
using Mawhiba.API.Gateway.Models;
using System.Text;
using System.Web;

namespace Mawhiba.API.Gateway.Services;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.EntityFrameworkCore;
// using HttpClientToCurl;
// Assuming Newtonsoft.Json is used for serialization
using Newtonsoft.Json;


public class APIService
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
    private readonly IConfiguration configuration;
    private readonly ContentServicesDbContext context;
    private ServiceHandler _serviceHandler;

    public APIService(ServiceHandlerParser serviceHandlerParser,
        IHttpContextAccessor httpContextAccessor,
        IWebHostEnvironment webHostEnvironment, 
        IConfiguration configuration,
        ContentServicesDbContext context)
    {


        this.serviceHandlerParser = serviceHandlerParser;
        this.httpContextAccessor = httpContextAccessor;
        this.webHostEnvironment = webHostEnvironment;
        this.configuration = configuration;
        this.context = context;
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
            ExceptionHandler.SaveLog("ExecuteCallAsync", context);


            var useJsonFile = configuration["UseJsonFiles"];
            _serviceHandler = useJsonFile == "True"?
                serviceHandlerParser.GetServiceByServiceId(_serviceHandler, serviceId, webHostEnvironment)
                : serviceHandlerParser.GetServiceByServiceId(_serviceHandler, serviceId, context);
            url = HttpUtility.UrlDecode(url);
            string fullUrl = $"{_serviceHandler.CurrentServiceInfo.BaseUrl.TrimEnd('/')}/{url}";

            context.ExceptionLogs.Add(new ExceptionLog { Id = Guid.NewGuid(), ExceptionText = fullUrl, ExceptionTime = DateTime.Now });
            context.SaveChanges();



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
                //string curl = _http.GenerateCurlInString(request);
                var response = await _http.SendAsync(request);

                string responseContent = await response.Content.ReadAsStringAsync();
                context.ExceptionLogs.Add(new ExceptionLog { Id = Guid.NewGuid(), ExceptionText = responseContent, ExceptionTime = DateTime.Now });
                context.SaveChanges();
                return await _serviceHandler.HandleResponseAsync(response);
            }
        }
        catch (Exception ex)
        {
            context.ExceptionLogs.Add(new ExceptionLog { Id = Guid.NewGuid(), ExceptionText = ex.ToString(), ExceptionTime = DateTime.Now });
            context.SaveChanges();

            return _serviceHandler.HandleException(ex);
        }
    }
}
