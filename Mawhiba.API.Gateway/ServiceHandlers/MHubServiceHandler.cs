


using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using System.Web;

namespace ApiGatewayService.API.ServiceHandlers;

public class MHubServiceHandler : ServiceHandler
{
    public MHubServiceHandler()
    {

    }

    public override HttpRequestMessage HandleRequest(HttpRequestMessage requestMessage, HttpRequest request)
    {
        base.HandleRequest(requestMessage, request);

        // Check if the URL contains the specified pattern
        if (requestMessage.RequestUri.AbsolutePath.Contains("GenerateUserToken", StringComparison.InvariantCultureIgnoreCase))
        {
            // Parse the Query String
            var query = HttpUtility.ParseQueryString(requestMessage.RequestUri.Query);
            var basePath = requestMessage.RequestUri.GetLeftPart(UriPartial.Path);


            // Initialize a list to hold other query parameters
            List<string> otherParams = new List<string>();


            // Extract the 'id' value and prepare to append other parameters
            string idValue = "";
            foreach (string key in query.AllKeys)
            {
                if (key == "userid")
                {
                    // Directly append the 'id' value to the path for specific handling
                    basePath += $"/{query[key]}";
                }
                else
                {
                    // For other parameters, add them to the list for later appending
                    otherParams.Add($"{key}={query[key]}");
                }
            }


            // Reconstruct the URL with other parameters
            string newUri = basePath;
            if (otherParams.Count > 0)
            {
                // Join other parameters with '&' and prepend with '?'
                newUri += "?" + string.Join("&", otherParams);
            }


            // Update the RequestUri of requestMessage
            requestMessage.RequestUri = new Uri(newUri);
        }

        return requestMessage;
    }

    public override async Task<APIResult> HandleResponseAsync(HttpResponseMessage response)
    {
        var apiResult = new APIResult
        {
            ResultMessage = response.ReasonPhrase ?? string.Empty,
            IsSuccess = false
        };

        string responseContent = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                options.Converters.Add(new ObjectDeserializer());

                // Try to deserialize as MawhibaHubAPIResult first
                MawhibaHubAPIResult? mawhibaHubResult = null;
                APIResult? apiResponse = null;

                try
                {
                    mawhibaHubResult = System.Text.Json.JsonSerializer.Deserialize<MawhibaHubAPIResult>(responseContent, options);
                }
                catch { /* Ignore and try APIResult */ }

                if (mawhibaHubResult != null && (mawhibaHubResult.List != null || mawhibaHubResult.Model != null))
                {
                    object? resultObject = mawhibaHubResult.Model ?? mawhibaHubResult.List;
                    if (resultObject is JsonElement jsonElement)
                    {
                        try
                        {
                            apiResult.ResultObject = System.Text.Json.JsonSerializer.Deserialize<object>(jsonElement.ToString()!);
                        }
                        catch
                        {
                            apiResult.ResultObject = null;
                        }
                    }
                    else
                    {
                        apiResult.ResultObject = resultObject;
                    }

                    apiResult.ResultMessage = mawhibaHubResult.Message;
                    apiResult.IsSuccess = mawhibaHubResult.Errors == null || mawhibaHubResult.Errors.IsNullOrEmpty();
                    return apiResult;
                }

                // If it’s not a MawhibaHubAPIResult, try parsing it as APIResult
                try
                {
                    apiResponse = System.Text.Json.JsonSerializer.Deserialize<APIResult>(responseContent, options);
                    if (apiResponse.ResultObject != null && apiResponse.ResultObject.GetType() == typeof(JsonElement))
                    {
                        try
                        {
                            apiResult.ResultObject = System.Text.Json.JsonSerializer.Deserialize(apiResponse.ResultObject.ToString()!, typeof(object));
                        }
                        catch
                        {

                            apiResult.ResultObject = null;
                        }
                    }
                    else
                    {
                        apiResult.ResultObject = apiResponse.ResultObject;
                    }

                    apiResult.ResultMessage = apiResponse.ResultMessage;
                    apiResult.IsSuccess = CurrentServiceInfo.IsSuccess(apiResponse.ResultCode ?? "");
                }
                catch { /* Ignore if both fail */ }

                if (apiResponse != null)
                {
                    return apiResponse;
                }
            }
            catch (System.Text.Json.JsonException ex)
            {
                apiResult.MoreDetails = $"JSON Deserialization Error: {ex.Message}";
            }
        }
        else
        {
            apiResult.MoreDetails = responseContent;
        }

        return apiResult;
    }

  

}

public class MawhibaHubAPIResult
{
    public object? Model { get; set; }
    public object? List { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, string> Errors { get; set; }
}
