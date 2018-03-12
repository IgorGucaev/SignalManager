using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.LocalEventStorage.Abstractions
{
    public class Signal
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public DateTime Timestamp { get; set; }
    }
}