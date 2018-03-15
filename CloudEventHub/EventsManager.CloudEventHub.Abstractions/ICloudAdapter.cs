using System;

namespace EventsManager.CloudEventHub.Abstractions
{
    public interface ICloudAdapter
    {
        void Enqueue(string jsonData);
    }
}
