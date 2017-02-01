using AzureStorage.Tables;
using Common;
using Common.Log;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.ProcessHealthMonitoring
{
    public class ServiceHealthMonitoringSettings
    {
        public string ConnectionString { get; set; }
        
    }

    public static class HealthServiceBinging
    {
        public static void BindServiceMonitoring(this IServiceCollection serviceCollection,
            ServiceHealthMonitoringSettings settings, ILog log)
        {

            serviceCollection.AddSingleton<IServiceHealthMonitoring>(
                new ServiceHealthMonitoringInAzureStorage(
                    new AzureTableStorage<HealthEntity>(settings.ConnectionString, "Monitoring", log))
            );

        }

    }

}
