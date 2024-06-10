using Mawhiba.API.Gateway.Models;

namespace Mawhiba.API.Gateway.Helpers
{
    public static class ExceptionHandler
    {
        public static void SaveLog(string log, ContentServicesDbContext context)
        {
            context.ExceptionLogs.Add(new ExceptionLog { Id = Guid.NewGuid(), ExceptionText = log, ExceptionTime = DateTime.Now });
            context.SaveChanges();
        }
    }
}
