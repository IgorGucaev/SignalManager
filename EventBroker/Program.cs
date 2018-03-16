using EventsGateway.Common;
using EventsManager.Abstractions;
using EventsManager.LocalEventStorage.Abstractions;
using EventsManager.LocalEventStorage.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using EventManager.CloudEventHub.Core;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace EventBroker
{
    class Program
    {
        public static IConfiguration Configuration { get; set; }

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            string deviceId = Configuration["devices:0:Name"];
            string deviceBinding = Configuration["devices:0:Binding"];


            IEventManager manager = new ServiceCollection()
             .AddSingleton<ILogger, DummyLogger>()
             .AddCacheContext(cacheSettings => {
                 cacheSettings.CreateDbScriptPath = Configuration["createDbScriptPath"];
                 cacheSettings.DbFilepath = Configuration["dbFilepath"];
             })
             .AddTransient<IUnitOfWork, BaseUnitOfWork>()
             .AddTransient<ISignalService, SignalService>()
             .AddCloudAdapter(senderSettings => {
                 senderSettings.AppendBinding(deviceId, deviceBinding);
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
                manager.RegisterEvent(Signal.New(deviceId, signal, DateTime.Now));
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