using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;

namespace Mawhiba.API.Gateway.V2.Controllers;

[Route("api/[controller]")]
[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    
    private readonly HttpClient http;
    private readonly ServiceHandler serviceHandler;
    private readonly ServicesCaller servicesCaller;
    private readonly int serviceId;
    private Dictionary<string, string> jsonData;
    
    public ApiControllerBase(ServicesCaller _servicesCaller, int serviceId,ServiceHandler handler)
    {
        this.http = _servicesCaller.Http;
        this.servicesCaller = _servicesCaller;
        this.serviceId = serviceId;
        ReadServiceJsonFile();
        
        http.BaseAddress = new Uri(servicesCaller.ServiceInfo.BaseUrl);
        this.serviceHandler = handler;
    }
   
    [HttpGet]
    [Route("/api/[controller]/get/{endpointkey}")]
    public virtual async Task<IActionResult> GetAsync(string endpointkey) // actionId OR Encrypted
    {
        try
        {
            //_serviceHandler = serviceHandlerParser.GetServiceByServiceId(_serviceHandler, ServiceId);
            string url = servicesCaller.ServiceInfo.Endpoints.First(it => it.Key == endpointkey).Route;
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            var request = serviceHandler.HandleRequest(requestMessage, Request);


            var response = await http.SendAsync(request);
            return Ok(await serviceHandler.HandleResponseAsync(response));
        }
        catch (Exception ex)
        {
            return BadRequest(serviceHandler.HandleException(ex));
        }
    }

    [HttpPost]
    [Route("/api/[controller]/post")]
    public virtual async Task<IActionResult> PostAsync(string url, object data)
    {
        try
        {
            //_serviceHandler = serviceHandlerParser.GetServiceByServiceId(_serviceHandler, ServiceId);
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            var request = serviceHandler.HandleRequest(requestMessage, Request);

            if (data != null)
            {
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                request.Content = content;
            }

            var response = await http.SendAsync(request);
            return Ok(await serviceHandler.HandleResponseAsync(response));
        }
        catch (Exception ex)
        {
            return BadRequest(serviceHandler.HandleException(ex));
        }
    }

    private void ReadServiceJsonFile()
    {
        var _hostingEnvironment = servicesCaller.WebHostEnvironment;// this.ServiceProvider.GetService<IHttpContextAccessor>();
        string file = Path.Combine(_hostingEnvironment.ContentRootPath, "wwwroot\\Services", $"{serviceId}.json");
        StreamReader reader = new(file);
        string content = reader.ReadToEnd();
        reader.Close();
        servicesCaller.ServiceInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<ServiceInfo>(content);
        //jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string,string>>(content);
    }
}

