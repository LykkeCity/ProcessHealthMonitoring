using System;
using System.Threading.Tasks;
using AzureStorage;
using Common;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.ProcessHealthMonitoring
{

    public class HealthEntity : TableEntity
    {
        public DateTime DateTime { get; set; }
        public string Version { get; set; }
        public static HealthEntity Create(string applicationName, string version)
        {
            return new HealthEntity
            {
                PartitionKey = "Monitoring",
                RowKey = applicationName,
                DateTime = DateTime.UtcNow,
                Version = version
            };
        }
    }

    public class ServiceHealthMonitoringInAzureStorage : IServiceHealthMonitoring
    {
        private string _version;
        private string _appName;

        private readonly INoSQLTableStorage<HealthEntity> _tableStorage;
        private readonly int _frequencyInSec;

        private DateTime _lastDateTime = DateTime.UtcNow.AddDays(-30);

        private void PopulateVersion()
        {
            _appName = Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationName;
            _version = Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationVersion;
        }

        public ServiceHealthMonitoringInAzureStorage(INoSQLTableStorage<HealthEntity> tableStorage, 
            int frequencyInSec = 30)
        {
            _tableStorage = tableStorage;
            _frequencyInSec = frequencyInSec;
            PopulateVersion();
        }

        public async Task HealthPingAsync()
        {

            var entity = HealthEntity.Create(_appName, _version);

            var now = DateTime.UtcNow;

            if ((now - _lastDateTime).TotalSeconds > _frequencyInSec)
                return;

            _lastDateTime = now;

            await _tableStorage.InsertAsync(entity);
        }

    }
}
