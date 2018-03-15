using EventsManager.Abstractions;
using EventsManager.CloudEventHub.Abstractions;
using EventsManager.CloudEventHub.Core;
using EventsManager.CloudEventHub.Gateway.Utils.MessageSender;
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
            string deviceId = "myDevice";

            IEventManager manager = new ServiceCollection()
             .AddTransient<IUnitOfWork, BaseUnitOfWork>()
             .AddTransient<ISignalService, SignalService>()
             .AddCloudAdapter(senderSettings => {
                 senderSettings.AppendBinding(deviceId, "HostName=technobee-infrastructure-testbed-01-iot-hub.azure-devices.net;DeviceId=myDevice;SharedAccessKey=kZTuwNbRvnmW5nz6XBvORD3GDo+K5bZdWCKlQBXACjA="); })
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

            foreach (string signal in data)
            {
                manager.RegisterEvent(Signal.New(deviceId, signal, DateTime.Now));
            }
            Console.WriteLine($"{testCount} events queued");

            Console.WriteLine("Wait...");
            Thread.Sleep(20 * 1000);
            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();
            manager.Stop();
            Environment.Exit(0);
        }
    }
}