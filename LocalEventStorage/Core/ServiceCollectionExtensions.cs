using EventsManager.CloudEventHub.Common;
using EventsManager.LocalEventStorage.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EventsManager.LocalEventStorage.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCacheContext(this IServiceCollection serviceCollection, Action<CacheSettings> setupAction = null)
        {
            return serviceCollection
                .AddTransient(sp => new CacheContext(setupAction?.CreateTargetAndInvoke()));
        }
    }
}