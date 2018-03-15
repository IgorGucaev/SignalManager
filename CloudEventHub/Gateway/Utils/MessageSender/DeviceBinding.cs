using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventsManager.CloudEventHub.Gateway.Utils.MessageSender
{
    public class DeviceBinding
    {
        public string DeviceId { get; private set; }
        public string GatewayHostName { get; private set; }

        public DeviceBinding(string deviceId, string gatewayHostName)
        {
            this.DeviceId = deviceId;
            this.GatewayHostName = gatewayHostName;
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
