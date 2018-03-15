using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.CloudEventHub.Gateway.Models
{
    public interface IQueuedItem
    {
        string GetDeviceId();
    }
}