namespace EventsGateway.Gateway
{
    using System.Collections.Generic;
    using EventsGateway.Common;
    using EventsGateway.Common.Threading;

    public abstract class EventProcessor
    {
        protected readonly ILogger _logger;

        protected EventProcessor(ILogger logger)
        {
            _logger = logger;
        }

        public delegate void EventBatchProcessedEventHandler(List<TaskWrapper> messages);

        public abstract bool Start();

        public abstract bool Stop(int timeout);

        public abstract void Process();

        protected ILogger Logger
        {
            get
            {
                return _logger;
            }
        }
    }
}
