using System;
using System.Reflection;
using System.Threading.Tasks;
using Common;

namespace Lykke.ProcessHealthMonitoring
{
    public class StillAlive
    {
        public string AppName { get; set; }

        public string Version { get; set; }

        public DateTime DateTime {get; set; }
        
    }

    public interface IServiceHealthMonitoring
    {
        Task HealthPingAsync();
    }

    public class ServiceHealthMonitoring : IServiceHealthMonitoring
    {
        private readonly IMessageProducer<StillAlive> _producer;

        private string _version;
        private string _appName;

        private void PopulateVersion(Assembly hostAssembly)
        {
            _appName = hostAssembly.FullName;

            try
            {
                _version =
                    hostAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

            }
            catch (Exception)
            {
                _version = "---";

            }
            
        }

        public ServiceHealthMonitoring(Assembly hostAssembly, IMessageProducer<StillAlive> producer)
        {
            PopulateVersion(hostAssembly);
            _producer = producer;
        }

        public async Task HealthPingAsync()
        {
            var item = new StillAlive
            {
                AppName = _appName,
                DateTime = DateTime.UtcNow,
                Version = _version
            };

            await _producer.ProduceAsync(item);
        }

    }
}
