using Mawhiba.API.Gateway.Models;
using Mawhiba.API.Gateway.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Runtime.InteropServices;

namespace Mawhiba.API.Gateway.Controllers;

[ApiController]
[Route("[controller]")]
public class ServiceCommandsController : ControllerBase
{
    private readonly APIService _apiService;
    //private readonly ContentServicesDbContext _context;

    public ServiceCommandsController(APIService apiService, ContentServicesDbContext context)
    {
        _apiService = apiService;
        //_context = context;
    }

    [HttpGet]
    [Route("/api/[controller]/get/")]
    public async Task<IActionResult> GetAsync(int serviceId, int userId, string? langCode, string? otherparams)
    {
        try
        {
            List<ServiceCommandItem> serviceCommands = new List<ServiceCommandItem>();

            string checkRegistrationStatus = "";
            string checkIfUserRegistered = "";

            switch (serviceId)
            {
                case 3: // Kangaroo
                    checkRegistrationStatus = "get?serviceId=3&url=Registrations/GetRegistrationOpenCloseStatus";
                    checkIfUserRegistered = $"get?serviceId=3&url=Registrations/GetRegistrationsToExam&id=-1&userID={userId}&schoolID=-1&examsSchoolID=-1";
                    break;
                case 6: // Bebras
                    checkRegistrationStatus = "get?serviceId=6&url=Registeration/IsCloseRegistration?systemPerionID=-1";
                    checkIfUserRegistered = $"get?serviceId=6&url=Registeration/GetRegisteration?userId={userId}";
                    break;
                case 7: // Olympics
                    checkRegistrationStatus = "get?serviceId=7&url=ExamsRegisteration/IsRegisterationClosed";
                    checkIfUserRegistered = $"get?serviceId=7&url=ExamsRegisteration/GetUserRegistrations?userId={userId}";
                    break;
                case 8: // Ibdaa
                    checkRegistrationStatus = "get?serviceId=8&url=Registrations/GetRegistrationOpenCloseStatus";
                    checkIfUserRegistered = $"get?serviceId=8&url=Registrations/GetUserRegistrations?userId={userId}";
                    break;
                case 9: // Sofraa
                    checkRegistrationStatus = "get?serviceId=9&url=/Registeration/CheckUserAndSettings";
                    checkIfUserRegistered = $"get?serviceId=9&url=/Registeration/GetRegisteration?userId={userId}";
                    break;
                default:
                    return BadRequest("Invalid service ID.");
            }

            // check if registeration open or close

            APIResult? result = await _apiService.CallAsync(16, checkRegistrationStatus, HttpMethod.Get, null);
            if (result != null && ConvertToBoolean(result.ResultObject) == false)
            {
                APIResult? isRegResult = await _apiService.CallAsync(16, checkIfUserRegistered, HttpMethod.Get, null);
                if (isRegResult != null && isRegResult.IsSuccess && isRegResult.ResultObject != null)
                {
                    serviceCommands.Add(new ServiceCommandItem { CommandTitle = "استعراض التسجيل", CommandParam = isRegResult.ResultObject });
                }
                else
                {
                    serviceCommands.Add(new ServiceCommandItem { CommandTitle = "التسجيل" });
                }
            }
            else
            {
                serviceCommands.Add(new ServiceCommandItem { CommandTitle = "التسجيل مغلق" });
            }

            return Ok(new APIResult { IsSuccess = true, ResultObject = serviceCommands });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }


    private bool ConvertToBoolean(object value)
    {
        if (value is bool boolValue)
        {
            return boolValue;
        }

        if (value is string strValue)
        {
            return strValue.Trim().Equals("false", StringComparison.OrdinalIgnoreCase) ? false : bool.TryParse(strValue, out bool parsedValue) && parsedValue;
        }

        return false; // Default to false if the value is not recognized
    }

}

public class ServiceCommandItem
{
    public string CommandTitle { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public int CommandCode { get; set; }
    public object? CommandParam { get; set; }
    //public string CommandParamJson { get; set; } = string.Empty;
}

public enum CommandType
{
    RegisterationClosed = 0,
    Registering = 1,
    ViewRegisteration = 2,
    Payment = 3,
    CancelRegisteration = 4,

}
