using Mawhiba.API.Gateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mawhiba.API.Gateway.Controllers;

[ApiController]
[Route("[controller]")]
public class GenericServiceController : ControllerBase
{
    private readonly IAPIService _apiService;


    public GenericServiceController(IAPIService apiService)
    {
        _apiService = apiService;
    }

    [HttpGet]
    [Route("/api/[controller]/get")]
    public async Task<IActionResult> GetAsync(int serviceId, string url) // actionId OR Encrypted
    {
        try
        {
            APIResult result = await _apiService.CallAsync(serviceId, url, HttpMethod.Get, null);
            return Ok(result.ResultObject);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    [Route("/api/[controller]/post")]
    public async Task<IActionResult> PostAsync(int serviceId, string url, object data)
    {
        try
        {
            APIResult result = await _apiService.CallAsync(serviceId, url, HttpMethod.Post, data);
            return Ok(result.ResultObject);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}


public class ExternalServiceRequest
{
    public string Url { get; set; }
    public HttpMethod Method { get; set; }
    public object Payload { get; set; }
    public string AuthorizationToken { get; set; }
    public string Host { get; set; }
}
