using ApiGatewayService.API.ServiceHandlers;

namespace Mawhiba.API.Gateway.Helpers;

public class ServiceHandlerParser
{
    public ServiceHandler GetServiceByServiceId(ServiceHandler service,int serviceId) //where T : ServiceHandler
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
        return result;
    }
}
