using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.CloudEventHub.Gateway.Utils.MessageSender
{
    public class IotHubSenderSettings
    {
        // public string IotHubConnectionString { get; private set; }
        public DeviceBindings Bindings { get; private set; }
            = new DeviceBindings();
        public TimeSpan StopTimeout { get; private set; } 

        public IotHubSenderSettings(){ }

        public void AppendBinding(string deviceId, string gatewayHostName)
        {
            this.Bindings.Add(new DeviceBinding(deviceId, gatewayHostName));
        }
    }
}