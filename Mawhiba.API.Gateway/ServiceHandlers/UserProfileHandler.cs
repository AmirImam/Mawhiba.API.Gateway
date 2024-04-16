


using System.Text.Json;
using System.Web;

namespace ApiGatewayService.API.ServiceHandlers;

public class UserProfileHandler : ServiceHandler
{
    public UserProfileHandler() { }

    public override HttpRequestMessage HandleRequest(HttpRequestMessage requestMessage, HttpRequest request)
    {
        base.HandleRequest(requestMessage, request);

        // Check if the URL contains the specified pattern
        if (requestMessage.RequestUri.AbsolutePath.Contains("GetFullDetailsForServicesByUserId", StringComparison.InvariantCultureIgnoreCase)
            || requestMessage.RequestUri.AbsolutePath.Contains("UpdateRegisterationEmailActivated", StringComparison.InvariantCultureIgnoreCase)
            || requestMessage.RequestUri.AbsolutePath.Contains("ActivateByMobile", StringComparison.InvariantCultureIgnoreCase)
            )
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
                if (key == "id")
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
            //ResultCode = ((int)response.StatusCode).ToString(),
            ResultMessage = response.ReasonPhrase ?? string.Empty,
            IsSuccess = false
        };

        string responseContent = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            try
            {
                var options = new JsonSerializerOptions();
                options.WriteIndented = true;
                options.Converters.Add(new ObjectDeserializer());
                var result = System.Text.Json.JsonSerializer.Deserialize<APIResult>(responseContent, options)!;

                if (result != null)
                {
                    if (result.ResultObject != null && result.ResultObject.GetType() == typeof(JsonElement))
                    {
                        try
                        {
                            apiResult.ResultObject = System.Text.Json.JsonSerializer.Deserialize(result.ResultObject.ToString()!, typeof(object));
                        }
                        catch
                        {

                            apiResult.ResultObject = null;
                        }
                    }
                    else
                    {
                        apiResult.ResultObject = result.ResultObject;
                    }

                    apiResult.ResultMessage = result.ResultMessage;
                    apiResult.IsSuccess = CurrentServiceInfo.IsSuccess(result.ResultCode);

                    // Special Cases
                    try
                    {
                        if (response.RequestMessage.RequestUri.AbsolutePath.Contains("RequestConfirmMobileNumberWithoutCodeEncrypted", StringComparison.InvariantCultureIgnoreCase)
                            || response.RequestMessage.RequestUri.AbsolutePath.Contains("RequestConfirmEmailAddressWithoutCodeEncrypted", StringComparison.InvariantCultureIgnoreCase))
                        {
                            apiResult.ResultObject = result.MoreDetails;
                        }
                    }
                    catch
                    {


                    }
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

}
