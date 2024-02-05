


using System.Text.Json;

namespace ApiGatewayService.API.ServiceHandlers;

public class UserProfileHandler : ServiceHandler
{
    public UserProfileHandler() { }

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
