using ApiGatewayService.API.ServiceHandlers;
using Mawhiba.API.Gateway.Models;

namespace Mawhiba.API.Gateway.Helpers;

public class ServiceHandlerParser
{
    public ServiceHandler GetServiceByServiceId(ServiceHandler service, int serviceId, ContentServicesDbContext context) //where T : ServiceHandler
    {
        ServiceHandler result = GetServiceHandler(serviceId);
        result.CurrentServiceInfo = JsonHelper.GetServiceInfo(serviceId, context);
        return result;
    }
    public ServiceHandler GetServiceByServiceId(ServiceHandler service, int serviceId, IWebHostEnvironment webHostEnvironment) //where T : ServiceHandler
    {
        ServiceHandler result = GetServiceHandler(serviceId);
        result.CurrentServiceInfo = JsonHelper.GetServiceInfo(serviceId, webHostEnvironment);
        return result;
    }

    private ServiceHandler GetServiceHandler(int serviceId)
    {
        ServiceHandler result;

        switch (serviceId)
        {
            case 0:
                result = new UserProfileHandler();
                break;

            case 6:
                result = new BebrasServiceHandler();
                break;

            case 3:
                result = new KangarooServiceHandler();
                break;

            default:
                result = new ServiceHandler();
                break;
        }
        return result;
    }
}
