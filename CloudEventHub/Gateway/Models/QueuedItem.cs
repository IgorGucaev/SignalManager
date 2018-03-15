namespace EventsGateway.Gateway
{
    using EventsManager.CloudEventHub.Gateway.Models;
    using System.Runtime.Serialization;

    [DataContract]
    public class QueuedItem : IQueuedItem
    {
        public readonly string DeviceId;

        [DataMember(Name = "serializedData")]
        public string JsonData { get; private set; }

        public QueuedItem(string deviceId, string jsonData)
        {
            DeviceId = deviceId;
            JsonData = jsonData;
        }

        public string GetDeviceId()
        {
            return DeviceId;
        }
    }
}
