using Mawhiba.API.Gateway.Models;
using Mawhiba.API.Gateway.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http.Extensions;
using System.Text;
using System.Text.RegularExpressions;

namespace Mawhiba.API.Gateway;

public class MyCustomMiddleware : IMiddleware
{
    private readonly ContentServicesDbContext dbcontext;

    public MyCustomMiddleware(
       ContentServicesDbContext context)
    {
        this.dbcontext = context;
    }


    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            ExceptionHandler.SaveLog("InvokeAsync", dbcontext);


            string querystring = context.Request.QueryString.Value.Substring(1, context.Request.QueryString.Value.Length - 1);
            var items = ExtractServiceIdAndUrl(querystring);

            int serviceId = int.Parse(items.Item1);
            string url = getvalues(items.Item2);

            string method = context.Request.Method.ToLower();
            APIService? apiService = context.RequestServices.GetService(typeof(APIService)) as APIService;
            if (apiService == null)
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
            ExceptionHandler.SaveLog(ex.ToString(), dbcontext);
            //await context.Response.WriteAsJsonAsync(ex);
        }
        //await next.Invoke(context);
    }

    static Tuple<string, string> ExtractServiceIdAndUrl(string input)
    {
        // Updated Regex pattern to extract serviceId and url
        var pattern = @"serviceId=(.*?)&url=(.*)";
        var match = Regex.Match(input, pattern);

        if (match.Success)
        {
            string serviceId = match.Groups[1].Value;
            string url = match.Groups[2].Value;
            return Tuple.Create(serviceId, url);
        }

        return null;
    }


    private string getvalues(string inputText)
    {
        string pattern = @"^(.*?)&";
        string result = Regex.Replace(inputText, pattern, "$1?");
        return result;
    }


   
}
