using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Mawhiba.API.Gateway.Models;

namespace Mawhiba.API.Gateway.Helpers;

public static class JsonHelper
{
    private static Dictionary<int, ServiceInfo> _serviceCache = new Dictionary<int, ServiceInfo>();

    public static ServiceInfo GetServiceInfo(int serviceId, ContentServicesDbContext context)
    {
        try
        {
            if (!_serviceCache.ContainsKey(serviceId))
            {
                _serviceCache = new Dictionary<int, ServiceInfo>();
                var services = context.GatewaySettings.ToList();
                foreach (var item in services)
                {
                    ServiceInfo? info = JsonConvert.DeserializeObject<ServiceInfo>(item.SettingJson);
                    if (info != null)
                    {
                        _serviceCache[item.ServiceId] = info;
                    }
                }
            }

            return _serviceCache[serviceId];
        }
        catch (Exception ex)
        {
            return null;
        }        

        
    }

    public static ServiceInfo GetServiceInfo(int serviceId,IWebHostEnvironment webHostEnvironment)
    {
        string file = Path.Combine(webHostEnvironment.ContentRootPath, "wwwroot\\Services", $"{serviceId}.json");
        using (StreamReader reader = new StreamReader(file))
        {
            string content = reader.ReadToEnd();
            ServiceInfo? info = JsonConvert.DeserializeObject<ServiceInfo>(content);
            if (info != null)
            {
                return _serviceCache[serviceId] = info;
            }
            return null;
        }
    }
}
