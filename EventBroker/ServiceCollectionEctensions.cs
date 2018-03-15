using EventsManager.CloudEventHub.Abstractions;
using EventsManager.CloudEventHub.Core;
using EventsManager.CloudEventHub.Gateway.Utils.MessageSender;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using EventsManager.CloudEventHub.Common;

namespace EventBroker
{

    public static class ServiceCollectionEctensions
    {
        public static IServiceCollection AddCloudAdapter(this IServiceCollection serviceCollection, Action<IotHubSenderSettings> setupAction = null)
        {
            return serviceCollection
                .AddSingleton<ICloudAdapter>(sp => new CloudAdapter(setupAction?.CreateTargetAndInvoke<IotHubSenderSettings>()));
        }
    }
}