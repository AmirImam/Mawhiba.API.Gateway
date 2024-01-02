using Microsoft.AspNetCore.Mvc;

namespace Mawhiba.API.Gateway.V2.Controllers;
public class KangarooController : ApiControllerBase
{
    public KangarooController(ServicesCaller servicesCaller,KangarooServiceHandler handler) : base(servicesCaller, 2, handler)
    {
    }
    public override Task<IActionResult> GetAsync(string url)
    {
        return base.GetAsync(url);
    }

}