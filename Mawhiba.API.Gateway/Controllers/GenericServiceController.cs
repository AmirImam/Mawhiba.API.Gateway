using Mawhiba.API.Gateway.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;

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

public class MyCustomMiddleware
{
    private readonly RequestDelegate next;


    public MyCustomMiddleware(RequestDelegate next)
    {
        this.next = next;
        
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            var queryParameters = context.Request.Query
             .SelectMany(q => q.Value, (col, value) => new { col.Key, value })
             .ToDictionary(arg => arg.Key, arg => arg.value.ToString());
            int serviceId = int.Parse(queryParameters["serviceId"]);
            string url = queryParameters["url"];
            string method = context.Request.Method.ToLower();
            var _apiService = context.RequestServices.GetService(typeof(IAPIService)) as IAPIService;
            var body = context.Request.Body;
            StringContent? stringContent = null;
            if (body != null)
            {
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                {
                    stringContent = new StringContent(await reader.ReadToEndAsync(), Encoding.UTF8, "application/json");
                }
            }



            APIResult result = await _apiService.CallAsync(serviceId, url, new HttpMethod(method), stringContent);
            await context.Response.WriteAsJsonAsync(result);
        }
        catch (Exception ex)
        {
           await context.Response.WriteAsJsonAsync(ex);
        }
        //await next.Invoke(context);
    }
}
