using ApiGatewayService.API.ServiceHandlers;

namespace Mawhiba.API.Gateway.Helpers;

public class ServiceHandlerParser
{
    public ServiceHandler GetServiceByServiceId(ServiceHandler service, int serviceId, IWebHostEnvironment webHostEnvironment) //where T : ServiceHandler
    {
        ServiceHandler result;

        switch (serviceId)
        {
            case 1:
                result = new BebrasServiceHandler();
                break;

            default:
                result = new ServiceHandler();
                break;
        }

        result.CurrentServiceInfo = JsonHelper.GetServiceInfo(serviceId, webHostEnvironment);

        return result;
    }
}
