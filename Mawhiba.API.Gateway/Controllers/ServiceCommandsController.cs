using Mawhiba.API.Gateway.Models;
using Mawhiba.API.Gateway.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Runtime.InteropServices;
using System.Text.Json;

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
    public async Task<IActionResult> GetAsyncGetAsync(int serviceId, int userId, string? langCode, string? otherparams)
    {
        try
        {
            var serviceCommands = new List<ServiceCommandItem>();
            ServicesEnum service = (ServicesEnum)serviceId;
            switch (service)
            {
                case ServicesEnum.TrainingCourses:
                    // Only training has this single command
                    serviceCommands.Add(new ServiceCommandItem
                    {
                        CommandTitle = "البرامج المتاحة للتسجيل",
                        CommandCode = (int)CommandType.Registering,
                        IsEnabled = true
                    });
                    break;

                case ServicesEnum.Tamayz:
                    // Only Tamayoz uses its own “GetAPIMobileSetting” endpoint
                    var tamayzUrl = $"get?serviceId={(int)service}" +
                                   "&url=/SettingsAndTermsAndConditions/GetAPIMobileSetting" +
                                   $"?userId={userId}&systemPeriodID=26";

                    var tamayzResult = await _apiService.CallAsync(
                        (int)ServicesEnum.ApiGateway,
                        tamayzUrl,
                        HttpMethod.Get,
                        null
                    );
                    serviceCommands.AddRange(GetCommandsForTamayoz(tamayzResult));
                    break;

                // All other “registration‐based” services
                case ServicesEnum.Kangaroo:
                case ServicesEnum.Bebras:
                case ServicesEnum.Mawhoob:
                case ServicesEnum.Ibda:
                case ServicesEnum.Sofaraa:
                    serviceCommands.AddRange(
                        await HandleRegistrationServiceAsync(service, userId)
                    );
                    break;

                default:
                    return BadRequest("Invalid service ID.");
            }

            return Ok(new APIResult
            {
                IsSuccess = true,
                ResultObject = serviceCommands
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    private async Task<IEnumerable<ServiceCommandItem>> HandleRegistrationServiceAsync(
        ServicesEnum service,
        int userId
    )
    {
        // URL‐fragments for each service
        var statusPaths = new Dictionary<ServicesEnum, string>
        {
            [ServicesEnum.Kangaroo] = "/Registrations/GetRegistrationOpenCloseStatus",
            [ServicesEnum.Ibda] = $"/Registeration/IsRegisterationClosed?userId={userId}",
            [ServicesEnum.Bebras] = "/Registeration/IsCloseRegistration?systemPerionID=-1",
            [ServicesEnum.Mawhoob] = "/ExamsRegisteration/IsRegisterationClosed",
            [ServicesEnum.Sofaraa] = "/Registeration/CheckUserAndSettings",
        };

        var userPaths = new Dictionary<ServicesEnum, string>
        {
            [ServicesEnum.Kangaroo] = $"/Registrations/GetRegistrationsToExam&id=-1&userID={userId}&schoolID=-1&examsSchoolID=-1",
            [ServicesEnum.Ibda] = $"/RegisterationV2/GetNewRegistrationDataV2?userId={userId}",
            [ServicesEnum.Bebras] = $"/Registeration/GetRegisteration?userId={userId}",
            [ServicesEnum.Mawhoob] = $"/ExamsRegisteration/GetUserRegistrations?userId={userId}",
            [ServicesEnum.Sofaraa] = $"/Registeration/GetRegisteration?userId={userId}",
        };

        var cmds = new List<ServiceCommandItem>();

        // 1) Is registration open?
        var statusUrl = $"get?serviceId={(int)service}&url={statusPaths[service]}";
        var statusResult = await _apiService.CallAsync(
            (int)ServicesEnum.ApiGateway,
            statusUrl,
            HttpMethod.Get,
            null
        );

        if (statusResult != null && getIfRegOpen((int)service, statusResult.ResultObject))
        {
            // 2) Has the user already registered?
            var userUrl = $"get?serviceId={(int)service}&url={userPaths[service]}";
            var regResult = await _apiService.CallAsync(
                (int)ServicesEnum.ApiGateway,
                userUrl,
                HttpMethod.Get,
                null
            );

            if (getIfUserRegistered((int)service, regResult))
            {
                // a) Show “View Registration”
                cmds.Add(new ServiceCommandItem
                {
                    CommandTitle = "استعراض التسجيل",
                    CommandCode = (int)CommandType.ViewRegisteration,
                    IsEnabled = true,
                    CommandParam = regResult.ResultObject
                });

                // b) Invoice logic
                var hasInvoice = GetJsonPropertyValue(regResult.ResultObject, "HasInvoice");
                var billsStatus = GetJsonPropertyValue(regResult.ResultObject, "BillsStatusID");

                if (hasInvoice.Equals("false", StringComparison.OrdinalIgnoreCase)
&& billsStatus != "2")
                {
                    cmds.Add(new ServiceCommandItem
                    {
                        CommandTitle = "الدفع",
                        CommandCode = (int)CommandType.Payment,
                        IsEnabled = true,
                        CommandParam = regResult.ResultObject
                    });
                }
                else if (hasInvoice.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    cmds.Add(new ServiceCommandItem
                    {
                        CommandTitle = "استعراض الفاتورة",
                        CommandCode = (int)CommandType.ViewPayment,
                        IsEnabled = true,
                        CommandParam = regResult.ResultObject
                    });
                }
            }
            else
            {
                // c) Offer to register
                cmds.Add(new ServiceCommandItem
                {
                    CommandTitle = "التسجيل",
                    CommandCode = (int)CommandType.Registering,
                    IsEnabled = true
                });
            }
        }
        else
        {
            // 3) Registration is closed
            cmds.Add(new ServiceCommandItem
            {
                CommandTitle = "التسجيل مغلق",
                CommandCode = (int)CommandType.RegisterationClosed,
                IsEnabled = false
            });
        }

        return cmds;
    }



    private bool getIfRegOpen(int serviceId, object value)
    {
        var finalValue = false;

        if (value is bool boolValue)
        {
            finalValue = boolValue;
        }
        else if (value is string strValue)
        {
            finalValue = strValue.Trim().Equals("false", StringComparison.OrdinalIgnoreCase) ? false : bool.TryParse(strValue, out bool parsedValue) && parsedValue;
        }
        else
        {
            finalValue = false;
        }


        switch (serviceId)
        {
            case 3: // Kangaroo
                return finalValue;
            case 6: // Bebras
                return !finalValue;
            case 7: // Olympics
                return !finalValue;
            case 8: // Ibdaa
                return !finalValue;
            case 9: // Sofraa
                return !finalValue;
            default:
                return !finalValue;
        }
    }

    private bool getIfUserRegistered(int serviceId, APIResult? isRegResult)
    {
        if (isRegResult == null) return false;


        switch (serviceId)
        {

            default:
                 //isRegResult.IsSuccess && isRegResult.ResultObject != null;

                if (!isRegResult.IsSuccess || isRegResult.ResultObject == null)
                    return false;

                // Check if it's an array and empty
                if (isRegResult.ResultObject.ToString() == "[]")
                    return false;

                return true;
        }
    }

    private string GetJsonPropertyValue(object jsonObject, string propertyName)
    {
        try
        {
            var jsonString = jsonObject.ToString();
            if (string.IsNullOrWhiteSpace(jsonString))
                return null;

            using var doc = JsonDocument.Parse(jsonString);
            var root = doc.RootElement;

            if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty(propertyName, out var prop))
            {
                return prop.ToString();
            }
        }
        catch
        {
            // Optionally log or handle parsing errors
        }

        return "";
    }

    private List<ServiceCommandItem> GetCommandsForTamayoz(APIResult? result)
    {
        if (result == null || result.ResultObject == null) return new List<ServiceCommandItem>();
        var commandsJson = result.ResultObject.ToString();

        var allCommands = System.Text.Json.JsonSerializer.Deserialize<List<TamayozCommand>>(commandsJson)
                          ?? new List<TamayozCommand>();

        // Filter only those with IsDisplayed == true and map to ServiceCommandItem
        var serviceCommands = allCommands
            .Where(c => c.IsDisplayed)
            .Select(c => new ServiceCommandItem
            {
                CommandTitle = c.Name,
                IsEnabled = c.IsDisplayed,
                CommandCode = c.ID == 4 ? (int)CommandType.ViewPayment : c.ID,
                // if you want to pass through the SystemPeriodID, you can:
                CommandParam = c.SystemPeriodID
            })
            .ToList();




        return serviceCommands;
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

public class TamayozCommand
{
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsDisplayed { get; set; }
    public int SystemPeriodID { get; set; }
}

public enum CommandType
{
    RegisterationClosed = 0,
    Registering = 1,
    ViewRegisteration = 2,
    Payment = 3,
    CancelRegisteration = 4,
    ViewPayment = 5,
    Notifications = 7,
    MyRequests = 11,

}
