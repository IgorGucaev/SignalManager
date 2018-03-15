using EventsGateway.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EventBroker
{
    public class DummyLogger : ILogger
    {
        public void Flush()
        {
            
        }

        public void LogError(string logMessage)
        {
            Trace.WriteLine(logMessage);
        }

        public void LogInfo(string logMessage)
        {
            Trace.WriteLine(logMessage);
        }
    }
}
