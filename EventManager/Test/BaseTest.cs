using EventsManager.Abstractions;
using EventsManager.CloudEventHub.Abstractions;
using EventsManager.CloudEventHub.Core;
using EventsManager.LocalEventStorage.Abstractions;
using EventsManager.LocalEventStorage.Core;
using Microsoft.Extensions.DependencyInjection;

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
                .AddTransient<ICloudAdapter, CloudAdapter>()
                .AddTransient<IEventManager, EventsManager.Core.EventManager>().BuildServiceProvider();
        }

        public T GetService<T>()
        {
            return _Provider.GetRequiredService<T>();
        }
    }
}
