using Mawhiba.API.Gateway.Models;
using Mawhiba.API.Gateway.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Runtime.InteropServices;

namespace Mawhiba.API.Gateway.Controllers;

[ApiController]
[Route("[controller]")]
public class GenericServiceController : ControllerBase
{
    private readonly APIService _apiService;
    //private readonly ContentServicesDbContext _context;

    public GenericServiceController(APIService apiService, ContentServicesDbContext context)
    {
        _apiService = apiService;
        //_context = context;
    }

    [HttpGet]
    [Route("/api/[controller]/get/{serviceId}/{url}")]
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

    [HttpPut]
    [Route("/api/[controller]/put")]
    public async Task<IActionResult> PutAsync(int serviceId, string url, object data)
    {
        try
        {
            APIResult result = await _apiService.CallAsync(serviceId, url, HttpMethod.Put, data);
            return Ok(result.ResultObject);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete]
    [Route("/api/[controller]/delete")]
    public async Task<IActionResult> DeleteAsync(int serviceId, string url, object data)
    {
        try
        {
            APIResult result = await _apiService.CallAsync(serviceId, url, HttpMethod.Delete, data);
            return Ok(result.ResultObject);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
