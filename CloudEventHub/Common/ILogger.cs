namespace EventsGateway.Common
{
    public interface ILogger
    {
        void Flush();

        void LogError(string logMessage);

        void LogInfo(string logMessage);
    }
}