﻿
namespace Mawhiba.API.Gateway;

public class ServiceHandler
{
    public ServiceHandler() { }

    public virtual HttpRequestMessage HandleRequest( HttpRequestMessage requestMessage,HttpRequest request)
    {
        //foreach (var header in request.Headers)
        //{
        //    requestMessage.Headers.Add(header.Key, new string[] { header.Value });
        //}
        //if(req)
        requestMessage = TryAddHeader(requestMessage, request, "authorization");
        requestMessage = TryAddHeader(requestMessage, request, "f_ur_453_x0");
        

        //if (authorization )
        //{
        //}
        return requestMessage;
    }


    public virtual async Task<APIResult> HandleResponseAsync(HttpResponseMessage response)
    {
        var result = new APIResult
        {
            ResultCode = ((int)response.StatusCode).ToString(),
            ResultMessage = response.ReasonPhrase
        };


        if (response.IsSuccessStatusCode)
        {
            result.ResultObject = System.Text.Json.JsonSerializer.Deserialize(await response.Content.ReadAsStringAsync(), typeof(object));// JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
        }
        else
        {
            result.MoreDetails = await response.Content.ReadAsStringAsync();
            // Additional handling for specific status codes can be added here
        }


        return result;
    }
    public virtual APIResult HandleException(Exception ex)
    {
        return new APIResult
        {
            ResultCode = "500", // Internal Server Error
            ResultMessage = "An error occurred",
            MoreDetails = ex.Message
        };
    }
    // Error codes

    private HttpRequestMessage TryAddHeader(HttpRequestMessage requestMessage, HttpRequest request,string headerkey)
    {
        var header = request.Headers[headerkey];
        if (header.Any())
        {
            requestMessage.Headers.Add(headerkey, header.ToString());
        }
        return requestMessage;
    }

}
