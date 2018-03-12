namespace EventsGateway.Gateway
{
    using EventsGateway.Common.Threading;

    //--//

    public interface IMessageSender<in T>
    {
        TaskWrapper SendMessage(T data);
        TaskWrapper SendSerialized(string jsonData);

        void Close();
    }
}
