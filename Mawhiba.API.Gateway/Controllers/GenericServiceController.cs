using Mawhiba.API.Gateway.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Mawhiba.API.Gateway.Controllers;

[ApiController]
[Route("[controller]")]
public class GenericServiceController : ControllerBase
{
    private readonly APIService _apiService;


    public GenericServiceController(APIService apiService)
    {
        _apiService = apiService;
    }

    [HttpGet]
    [Route("/api/[controller]/get")]
    public async Task<IActionResult> GetAsync(int serviceId, string url) // actionId OR Encrypted
    {
        try
        {
            APIResult? result = await _apiService.CallAsync(serviceId, url, HttpMethod.Get, null);
            return Ok(result);
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
