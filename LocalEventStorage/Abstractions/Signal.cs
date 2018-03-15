using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EventsManager.LocalEventStorage.Abstractions
{
    public class Signal : IdentifiableEntity<string>
    {
        public string DeviceId { get; set; }
        public string Data { get; set; }
        public DateTime Timestamp { get; set; }

        protected Signal()
        {
            this.Id = DateTime.Now.ToString("yyMMddhhmmssfff") + "_" + Guid.NewGuid().ToString().Substring(15, 20);
        }

        public static Signal New(string deviceId, string data = "", DateTime? timestamp = null)
        {
            if (String.IsNullOrWhiteSpace(deviceId))
                throw new ArgumentException("Device Id must be specified!");

            if (String.IsNullOrWhiteSpace(data))
                throw new ArgumentException("Signal data must be specified!");

            return new Signal() { DeviceId = deviceId, Data = data, Timestamp = timestamp.HasValue ? timestamp.Value : DateTime.Now };
        }
    }
}