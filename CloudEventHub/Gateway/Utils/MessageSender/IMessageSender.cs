namespace EventsGateway.Gateway
{
    using EventsGateway.Common.Threading;

    public interface IMessageSender<in T>
    {
        TaskWrapper SendMessage(string deviceId, T data);
        TaskWrapper SendSerialized(string deviceId, string jsonData);

        void Close();
    }
}
