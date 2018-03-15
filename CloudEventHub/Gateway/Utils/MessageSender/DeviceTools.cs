using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventsManager.CloudEventHub.Gateway.Utils.MessageSender
{
    public class DeviceClientExt
    {
        public string DeviceId { get; set; }
        public DeviceClient Client { get; set; }

        public DeviceClientExt(string deviceId, DeviceClient client)
        {
            DeviceId = deviceId;
            Client = client;
        }

        public override string ToString()
        {
            return DeviceId;
        }
    }

    public class DeviceClientExtCollection : List<DeviceClientExt>
    {
        public DeviceClient this[string deviceId]
        {
            get
            {
                return this.FirstOrDefault(x => x.DeviceId == deviceId.Trim())?.Client;
            }
        }

        public void Append(string deviceId, DeviceClient client)
        {
            Add(new DeviceClientExt(deviceId, client));
        }

        public void Close()
        {
            foreach (var device in this)
            {
                device.Client.Dispose();
                device.Client = null;
            }
        }

        public override string ToString()
        {
            return Count.ToString();
        }
    }
    
    public class DeviceBinding
    {
        public string DeviceId { get; private set; }
        public string GatewayHostName { get; private set; }

        public DeviceBinding(string deviceId, string gatewayHostName)
        {
            DeviceId = deviceId;
            GatewayHostName = gatewayHostName;
        }

        public override string ToString()
        {
            return $"{DeviceId} [{GatewayHostName}]";
        }
    }

    public class DeviceBindings : List<DeviceBinding>
    {
        public DeviceBinding this[string DeviceId]
        {
            get
            {
                return this.First(x => x.DeviceId.Equals(DeviceId, StringComparison.InvariantCultureIgnoreCase));
            }
        }
    }
}
