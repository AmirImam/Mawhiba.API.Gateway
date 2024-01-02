

namespace ApiGatewayService.API.ServiceHandlers;

public class KangarooServiceHandler : ServiceHandler
{
    public override Task<APIResult> HandleResponseAsync(HttpResponseMessage response)
    {
        return base.HandleResponseAsync(response);
    }

    public override APIResult HandleException(Exception ex)
    {
        return base.HandleException(ex);
    }

}