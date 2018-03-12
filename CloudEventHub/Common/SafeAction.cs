namespace EventsGateway.Common
{
    using System;

    //--//

    public class SafeAction<TParam>
    {
        private readonly Action<TParam> _action;
        private readonly ILogger _logger;

        //--//

        public SafeAction(Action<TParam> action, ILogger logger)
        {
            _action = action;
            _logger = SafeLogger.FromLogger(logger);
        }

        public void SafeInvoke(TParam obj)
        {
            try
            {
                _action(obj);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception in task: " + ex.StackTrace);
                _logger.LogError("Message in task: " + ex.Message);
            }
        }
    }

    public class SafeAction
    {
        private readonly Action _action;
        private readonly ILogger _logger;

        //--//

        public SafeAction(Action action, ILogger logger)
        {
            _action = action;
            _logger = SafeLogger.FromLogger(logger);
        }

        public void SafeInvoke()
        {
            try
            {
                _action();
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception in task: " + ex.StackTrace);
                _logger.LogError("Message in task: " + ex.Message);
            }
        }
    }
}
