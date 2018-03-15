using EventsManager.CloudEventHub.Abstractions;
using EventsManager.CloudEventHub.Gateway.Utils.MessageSender;
using Microsoft.Extensions.DependencyInjection;
using System;
using EventsManager.CloudEventHub.Common;
using EventsManager.CloudEventHub.Core;

namespace EventManager.CloudEventHub.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCloudAdapter(this IServiceCollection serviceCollection, Action<IotHubSenderSettings> setupAction = null)
        {
            return serviceCollection
                .AddSingleton<ICloudAdapter>(sp => new CloudAdapter(setupAction?.CreateTargetAndInvoke<IotHubSenderSettings>()));
        }
    }
}