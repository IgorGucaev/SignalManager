using EventsGateway.Common;
using EventsGateway.Common.Threading;
using EventsGateway.Gateway;
using EventsManager.CloudEventHub.Abstractions;
using EventsManager.CloudEventHub.Gateway.Utils.MessageSender;
using System;
using System.Collections.Generic;
using System.Threading;

namespace EventsManager.CloudEventHub.Core
{
    public class CloudAdapter : ICloudAdapter, IDisposable
    {
        private const int STOP_TIMEOUT_MS = 5000; // ms

        private readonly ILogger _logger;
        private readonly AutoResetEvent _completed;
        private readonly GatewayQueue<QueuedItem> _gatewayQueue;
        private readonly IMessageSender<SensorDataContract> _sender;
        private readonly BatchSenderThread<QueuedItem, SensorDataContract> _batchSenderThread;
        private readonly Func<string, QueuedItem> _gatewayTransform;
        private GatewayService service;
         
        public CloudAdapter(IotHubSenderSettings settings)
        {
            _gatewayQueue = new GatewayQueue<QueuedItem>();
            _sender = new IotHubSender<SensorDataContract>(settings, _logger); // new RabbitMQSender<SensorDataContract>(_logger/*, "address"*/); // MockSender
            _batchSenderThread = new BatchSenderThread<QueuedItem, SensorDataContract>(
                _gatewayQueue,
                _sender,
                dataTransform: null,
                serializedData: m => (m == null) ? null : m.JsonData,
                logger: _logger
                );

            Func<string, SensorDataContract> transform = (m => DataTransforms.SensorDataContractFromString(m, _logger));

            _gatewayTransform = (m => DataTransforms.QueuedItemFromSensorDataContract(transform(m)));

            service = PrepareGatewayService();
        }

        private GatewayService PrepareGatewayService()
        {
            _batchSenderThread.Start();

            GatewayService service = new GatewayService(
                _gatewayQueue,
                _batchSenderThread,
                _gatewayTransform
            );

            service.Logger = _logger;
            service.OnDataInQueue += DataInQueue;

            _batchSenderThread.OnEventsBatchProcessed += EventBatchProcessed;

            return service;
        }

        protected virtual void DataInQueue(QueuedItem data)
        {
            // LORENZO: test behaviours such as accumulating data an processing in batch
            // as it stands, we are processing every event as it comes in

            _batchSenderThread.Process();
        }

        protected virtual void EventBatchProcessed(List<TaskWrapper> messages)
        {
            // LORENZO: test behaviours such as waiting for messages to be delivered or re-transmission

            if (messages == null || messages.Count == 0)
            {
                return;
            }

            foreach (TaskWrapper t in messages)
            {
                _logger.LogInfo(String.Format("task {0} status is '{1}'", t.Id, t.Status.ToString()));
            }

            TaskWrapper.BatchWaitAll(((List<TaskWrapper>)messages).ToArray());

            foreach (TaskWrapper t in messages)
            {
                _logger.LogInfo(String.Format("task {0} status is '{1}'", t.Id, t.Status.ToString()));
            }
        }

        public void Enqueue(string jsonData)
        {
            try
            {
                service.Enqueue(jsonData);
            }
            catch (Exception ex)
            {
                _logger.LogError("exception caught: " + ex.StackTrace);
            }
        }

        public void Dispose()
        {
            _batchSenderThread.Stop(STOP_TIMEOUT_MS);
            _sender.Close();
        }
    }
}