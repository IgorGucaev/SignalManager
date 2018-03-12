using EventManager.Abstractions;
using EventsGateway.Gateway;
using EventsStore.Abstractions;
using System;
using System.Collections.Generic;

namespace EventManager.Core
{
    internal class EventSerializedDragAndDrop : IEventDragAndDrop<string>
    {
        private ISignalCache<string> _Cache;
        private IGatewayService _Gateway;

        public EventSerializedDragAndDrop(ISignalCache<string> cache, IGatewayService gateway)
        {
            _Cache = cache;
            _Gateway = gateway;
        }

        public IEnumerable<string> Drag(DateTime fromIncluded, DateTime toExcluded)
        {
            return _Cache.GetSignalsSerialized(fromIncluded, toExcluded);
        }

        public bool Drop(IEnumerable<string> events)
        {
            //_Gateway.Enqueue();
            return false;
        }
    }
}
