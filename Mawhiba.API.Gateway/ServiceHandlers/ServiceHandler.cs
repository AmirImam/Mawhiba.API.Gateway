
using Mawhiba.API.Gateway.Helpers;
using Microsoft.Extensions.Hosting;
using System.Reflection.PortableExecutable;
using System.Text.Json;
using System.Xml.Linq;

namespace Mawhiba.API.Gateway;

public class ServiceHandler
{
    public ServiceHandler() { }
    public ServiceInfo CurrentServiceInfo { get; set; }
    public virtual HttpRequestMessage HandleRequest(HttpRequestMessage requestMessage, HttpRequest request)
    {
        //Dictionary<string, string> headers = ExtractIncomingHeaders(request);
        
        Dictionary<string, string> headers = ExtractIncomingHeaders(request);

        if (headers != null)
        {
            foreach (var header in headers)
            {
                requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        return requestMessage;
    }




    public virtual async Task<APIResult> HandleResponseAsync(HttpResponseMessage response)
    {
        var apiResult = new APIResult
        {
            //ResultCode = ((int)response.StatusCode).ToString(),
            ResultMessage = response.ReasonPhrase ?? string.Empty,
            IsSuccess = false
        };

        string responseContent = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            try
            {


                var result = System.Text.Json.JsonSerializer.Deserialize<APIResult>(responseContent);

                //var result =  JsonConvert.DeserializeObject<APIResult>(responseContent);
                //var result = JsonConvert.DeserializeObject<dynamic>(responseContent)!;
                //var str = System.Text.Json.JsonSerializer.Serialize(result.ResultObject);

                if (result != null)
                {
                    if (result.ResultObject != null)
                    {
                        try
                        {
                            apiResult.ResultObject = System.Text.Json.JsonSerializer.Deserialize(result.ResultObject.ToString()!, typeof(object));
                        }
                        catch (System.Text.Json.JsonException ex)
                        {
                            if (ex.ToString().Contains("is an invalid start of a value."))
                            {
                                apiResult.ResultObject = result.ResultObject.ToString();
                            }
                            else
                            {
                                apiResult.ResultObject = null;
                            }
                        }
                        catch
                        {
                            apiResult.ResultObject = null;
                        }
                    }

                    apiResult.ResultMessage = result.ResultMessage;
                    apiResult.IsSuccess = CurrentServiceInfo.IsSuccess(result.ResultCode);
                }
            }
            catch (System.Text.Json.JsonException ex)
            {
                apiResult.MoreDetails = $"JSON Deserialization Error: {ex.Message}";
            }
        }
        else
        {
            apiResult.MoreDetails = await response.Content.ReadAsStringAsync();
        }

        return apiResult;
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

    private HttpRequestMessage TryAddHeader(HttpRequestMessage requestMessage, HttpRequest request, string headerkey)
    {
        var header = request.Headers[headerkey];
        if (header.Any())
        {
            requestMessage.Headers.Add(headerkey, header.ToString());
        }
        return requestMessage;
    }


    private Dictionary<string, string> ExtractIncomingHeaders(HttpRequest request)
    {
        try
        {
            var headers = new Dictionary<string, string>();

            var incomingHeaders = request.Headers;

            if (incomingHeaders == null)
            {
                return null;
            }

            // Check for "Authorization" and "F_ur_453_x0" headers and add them if present
            if (incomingHeaders.ContainsKey("Authorization"))
            {
                headers.Add("Authorization", incomingHeaders["Authorization"].ToString());
            }

            if (incomingHeaders.ContainsKey("F_ur_453_x0"))
            {
                headers.Add("F_ur_453_x0", incomingHeaders["F_ur_453_x0"].ToString());
            }

            return headers;
        }
        catch
        {
            return null;
        }
    }
    private Dictionary<string, string> ExtractIncomingHeadersForTest(HttpRequest request)
    {
        try
        {

            string userTokenValue = "72PUG8/rVNpqtWd57Zuv8kg8C5Q1jgcpVEX7ySvoYQllMXgTiAIXnhGLvn8Qi6xFREovzu0bQIWHkHwRj806oA==";
            string authToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6Ik1hd2hpYmFBUFBNb2JpbGUiLCJuYmYiOjE3MTE4Njg2ODgsImV4cCI6MTcxNDQ2MDY4OCwiaWF0IjoxNzExODY4Njg4LCJpc3MiOiJNYXdoaWJhIiwiYXVkIjoiTW9iaWxlQVBQIn0.lKEOfG1_Fvk0eI5S0HOvlkFS8IzzJAWuU9awYpsc-A0";

            var headers = new Dictionary<string, string>();

            var incomingHeaders = request.Headers;

            if (incomingHeaders == null)
            {
                return null;
            }

            headers.Add("Authorization", authToken);
            headers.Add("F_ur_453_x0", userTokenValue);

            return headers;
        }
        catch
        {
            return null;
        }
    }

}
public class ApiResultObject
{
    public string ResultCode { get; set; }
    public string ResultMessage { get; set; }
    public object MoreDetails { get; set; }
    public object ValidatonDictionary { get; set; }
    public int ResultObject { get; set; }
}