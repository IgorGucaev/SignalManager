namespace EventsGateway.Common
{
    using NLog;

    public class NLogEventLogger : ILogger
    {
        #region Singleton implementation

        private static readonly object _syncRoot = new object();

        //--//

        private static ILogger _NLogEventLoggerInstance;
        private static NLog.Logger _NLog;

        //--//

        public static ILogger Instance
        {
            get
            {
                if (_NLogEventLoggerInstance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_NLogEventLoggerInstance == null)
                        {
                            _NLogEventLoggerInstance = new NLogEventLogger();
                        }
                    }
                }

                return _NLogEventLoggerInstance;
            }
        }

        private NLogEventLogger()
        {
            _NLog = LogManager.GetCurrentClassLogger();
        }

        #endregion

        public void Flush()
        {
            LogManager.Flush();
        }

        public void LogError(string logMessage)
        {
            _NLog.Error(logMessage);
        }

        public void LogInfo(string logMessage)
        {
            _NLog.Info(logMessage);
        }
    }
}
