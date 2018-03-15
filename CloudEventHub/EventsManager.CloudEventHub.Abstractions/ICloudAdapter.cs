using System;

namespace EventsManager.CloudEventHub.Abstractions
{
    public interface ICloudAdapter : IDisposable
    {
        void Enqueue(string deviceId, string jsonData);
    }
}
