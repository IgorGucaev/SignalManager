using EventsGateway.Common;
using EventsManager.Abstractions;
using EventsManager.LocalEventStorage.Abstractions;
using EventsManager.LocalEventStorage.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;

namespace EventBroker
{
    class Program
    {
        static void Main(string[] args)
        {
            string fooDevice = "fooDevice";
            string barDevice = "barDevice";

            IEventManager manager = new ServiceCollection()
             .AddSingleton<ILogger, DummyLogger>()
             .AddTransient<IUnitOfWork, BaseUnitOfWork>()
             .AddTransient<ISignalService, SignalService>()
             .AddCloudAdapter(senderSettings => {
                 senderSettings.AppendBinding(fooDevice, "HostName=technobee-infrastructure-testbed-01-iot-hub.azure-devices.net;DeviceId=myDevice;SharedAccessKey=kZTuwNbRvnmW5nz6XBvORD3GDo+K5bZdWCKlQBXACjA=");
                 senderSettings.AppendBinding(barDevice, "HostName=technobee-infrastructure-testbed-01-iot-hub.azure-devices.net;DeviceId=barDevice;SharedAccessKey=kZTuwNbRvnmW5nz6XBvORD3GDo+K5bZdWCKlQBXACjA=");
             })
             .AddTransient<IEventManager, EventsManager.Core.EventManager>()
             .BuildServiceProvider()
             .GetRequiredService<IEventManager>();

            manager.Start();
            Console.WriteLine("Event manager started");
            Console.WriteLine("Enter test size (0-1000)");

            int testCount = 100;

            int.TryParse(Console.ReadLine(), out testCount);

            List<string> data = new List<string>();
            for (int i = 0; i < testCount; i++)
                data.Add("{ guid: \"" + Guid.NewGuid().ToString() + "\"}");

            bool flag = true;

            foreach (string signal in data)
            {
                flag = !flag;
                manager.RegisterEvent(Signal.New(flag ? fooDevice : barDevice, signal, DateTime.Now));
            }
            Console.WriteLine($"{testCount} events queued");

            Console.WriteLine("Wait...");
            Thread.Sleep(20 * 1000);
            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();
            manager.Stop();
            manager.Dispose();
            Environment.Exit(0);
        }
    }
}