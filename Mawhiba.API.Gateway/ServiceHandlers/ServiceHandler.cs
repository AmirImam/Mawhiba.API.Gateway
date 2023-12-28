
namespace Mawhiba.API.Gateway;

public class ServiceHandler
{
    public ServiceHandler() { }

    public virtual HttpRequestMessage HandleRequest(HttpRequestMessage requestMessage)
    {
        requestMessage.Headers.Add("Authorization", "your_authorization_token");
        requestMessage.Headers.Add("Host", "your_host_value");

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
            result.ResultObject = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
        }
        else
        {
            result.MoreDetails = await response.Content.ReadAsStringAsync();
            // Additional handling for specific status codes can be added here
        }


        return result;
    }
    public APIResult HandleException(Exception ex)
    {
        return new APIResult
        {
            ResultCode = "500", // Internal Server Error
            ResultMessage = "An error occurred",
            MoreDetails = ex.Message
        };
    }
    // Error codes

}
