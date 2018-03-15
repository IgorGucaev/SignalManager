using EventsGateway.Common;
using System.Diagnostics;

namespace EventManager.CloudEventHub
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
