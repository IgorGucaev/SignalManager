namespace  EventsGateway.Gateway.Utils.MessageSender
{
    using EventsGateway.Common;
    using EventsGateway.Common.Threading;
    using EventsGateway.Gateway;
    using Newtonsoft.Json;
    using RabbitMQ.Client;
    using System;
    using System.Text;

    public class RabbitMQSender<T> : IMessageSender<T>
    {
        const string QUEUE_NAME = "SensorsData";
        private static readonly string _logMesagePrefix = "MessageSender error. ";

        private IConnection connection = null;
        private IModel channel = null;

        public ILogger Logger
        {
            private get;
            set;
        }

        public RabbitMQSender(ILogger logger, string hostName = "localhost")
        {
            Logger = SafeLogger.FromLogger(logger);

            var factory = new ConnectionFactory() { HostName = hostName };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.QueueDeclare(queue: QUEUE_NAME,
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);
        }

        public TaskWrapper SendMessage(T data)
        {
            TaskWrapper result = null;

            try
            {
                if (data == null)
                {
                    return default(TaskWrapper);
                }

                string jsonData = JsonConvert.SerializeObject(data);

                result = PrepareAndSend(jsonData);
            }
            catch (Exception ex)
            {
                Logger.LogError(_logMesagePrefix + ex.Message);
            }

            return result;

        }

        public TaskWrapper SendSerialized(string jsonData)
        {
            TaskWrapper result = null;

            try
            {
                if (String.IsNullOrEmpty(jsonData))
                {
                    return default(TaskWrapper);
                }

                result = PrepareAndSend(jsonData);
            }
            catch (Exception ex)
            {
                Logger.LogError(_logMesagePrefix + ex.Message);
            }

            return result;
        }

        public void Close()
        {
            channel.Close();
            connection.Close();
        }

        private TaskWrapper PrepareAndSend(string jsonData)
        {
            byte[] body = Encoding.UTF8.GetBytes(jsonData);

            var sh = new SafeAction<byte[]>(m => channel.BasicPublish(exchange: "",
                                 routingKey: QUEUE_NAME,
                                 basicProperties: null,
                                 body: m), Logger);

            return TaskWrapper.Run(() => sh.SafeInvoke(body));
        }
    }
}
