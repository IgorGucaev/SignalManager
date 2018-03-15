using EventsManager.Abstractions;
using EventsManager.CloudEventHub.Abstractions;
using EventsManager.CloudEventHub.Core;
using EventsManager.LocalEventStorage.Abstractions;
using EventsManager.LocalEventStorage.Core;
using Microsoft.Extensions.DependencyInjection;
using EventManager.CloudEventHub.Core;
using EventManager.CloudEventHub;
using EventsGateway.Common;

namespace EventsManager.Test
{
    public class BaseTest
    {
        private ServiceProvider _Provider;

        public BaseTest()
        {
            this._Provider = new ServiceCollection()
                .AddTransient<IUnitOfWork, BaseUnitOfWork>()
                .AddTransient<ISignalService, SignalService>()
                .AddTransient<ILogger, DummyLogger>()
                  .AddCloudAdapter(senderSettings => {
                      senderSettings.AppendBinding("fooDevice", "HostName=technobee-infrastructure-testbed-01-iot-hub.azure-devices.net;DeviceId=myDevice;SharedAccessKey=kZTuwNbRvnmW5nz6XBvORD3GDo+K5bZdWCKlQBXACjA=");
                  })
                .AddTransient<IEventManager, EventsManager.Core.EventManager>().BuildServiceProvider();
        }

        public T GetService<T>()
        {
            return _Provider.GetRequiredService<T>();
        }
    }
}
