using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mawhiba.API.Gateway.Helpers
{
    public static class JsonHelper
    {
        private static Dictionary<int, ServiceInfo> _serviceCache = new Dictionary<int, ServiceInfo>();

        public static ServiceInfo GetServiceInfo(int serviceId, IWebHostEnvironment webHostEnvironment)
        {
            if (!_serviceCache.ContainsKey(serviceId))
            {
                string file = Path.Combine(webHostEnvironment.ContentRootPath, "wwwroot\\Services", $"{serviceId}.json");
                using (StreamReader reader = new StreamReader(file))
                {
                    string content = reader.ReadToEnd();
                    ServiceInfo? info = JsonConvert.DeserializeObject<ServiceInfo>(content);
                    if (info != null)
                    {
                        _serviceCache[serviceId] = info;
                    }
                }
            }

            return _serviceCache[serviceId];
        }
    }
}
