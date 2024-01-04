using Microsoft.AspNetCore.Mvc;

namespace Mawhiba.API.Gateway.V2.Controllers;
public class BebrasController : ApiControllerBase
{
    public BebrasController(ServicesCaller servicesCaller,BebrasServiceHandler handler) : base(servicesCaller,1,handler)
    {
    }

    public override Task<IActionResult> GetAsync(string endpointkey)
    {
        return base.GetAsync(endpointkey);
    }
}
