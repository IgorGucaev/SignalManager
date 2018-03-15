namespace EventsGateway.Common
{
    using System;

    /// <summary>
    /// Do save operation (not throws exceptions but log it)
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class SafeFunc<TResult>
    {
        private readonly Func<TResult> _function;
        private readonly ILogger _logger;

        public SafeFunc(Func<TResult> function, ILogger logger)
        {
            _function = function;
            _logger = SafeLogger.FromLogger(logger);
        }

        public TResult SafeInvoke()
        {
            try
            {
                return _function();
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception in task: " + ex.StackTrace);
            }

            return default(TResult);
        }
    }
}