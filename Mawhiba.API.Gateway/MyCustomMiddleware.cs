using Mawhiba.API.Gateway.Services;
using System.Text;

namespace Mawhiba.API.Gateway;

public class MyCustomMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            var queryParameters = context.Request.Query
             .SelectMany(q => q.Value, (col, value) => new { col.Key, value })
             .ToDictionary(arg => arg.Key, arg => arg.value.ToString());
            int serviceId = int.Parse(queryParameters["serviceId"]);
            string url = queryParameters["url"];
            string method = context.Request.Method.ToLower();
            APIService? apiService = context.RequestServices.GetService(typeof(APIService)) as APIService;
            if(apiService == null)
            {
                await context.Response.WriteAsJsonAsync("Service not found");
                return;
            }
            var body = context.Request.Body;
            StringContent? stringContent = null;
            if (body != null)
            {
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                {
                    stringContent = new StringContent(await reader.ReadToEndAsync(), Encoding.UTF8, "application/json");
                }
            }

            APIResult? result = await apiService.CallAsync(serviceId, url, new HttpMethod(method), stringContent);
            await context.Response.WriteAsJsonAsync(result);
        }
        catch (Exception ex)
        {
            await context.Response.WriteAsJsonAsync(ex);
        }
        //await next.Invoke(context);
    }
}
