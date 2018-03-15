using EventsManager.Abstractions;
using EventsManager.LocalEventStorage.Abstractions;
using EventsManager.LocalEventStorage.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.LocalEventStorage.Test
{
    public class BaseTest
    {
        private ServiceProvider _Provider;

        public BaseTest()
        {
            this._Provider = new ServiceCollection()
                .AddTransient<IUnitOfWork, BaseUnitOfWork>()
                .AddTransient<ISignalService, SignalService>()
                .BuildServiceProvider();
        }

        public T GetService<T>()
        {
            return _Provider.GetRequiredService<T>();
        }
    }
}
