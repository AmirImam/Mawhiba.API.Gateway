






using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Web;

namespace ApiGatewayService.API.ServiceHandlers;

public class YassirServiceHandler : ServiceHandler
{
    public YassirServiceHandler()
    {

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



                var requestUrl = response.RequestMessage?.RequestUri?.ToString();
                if (!requestUrl.Contains("EducationInformation/GetStudentEducationInformation", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result = System.Text.Json.JsonSerializer.Deserialize<APIResult>(responseContent, options);
                    if (result.ResultObject is JsonElement element)
                    {
                        try
                        {
                            apiResult.ResultObject = System.Text.Json.JsonSerializer.Deserialize<object>(element.ToString()!);
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
                    apiResult.IsSuccess = CurrentServiceInfo.IsSuccess(result.ResultCode ?? "");
                }
                else
                {
                    apiResult.ResultObject = responseContent;
                    apiResult.IsSuccess = true;
                }
            }
            catch (System.Text.Json.JsonException ex)
            {
                apiResult.MoreDetails = $"JSON Deserialization Error: {ex.Message}";
                apiResult.ResultObject = responseContent; // fallback to string form of response
            }
        }
        else
        {
            apiResult.MoreDetails = responseContent;
        }

        return apiResult;
    }

}

