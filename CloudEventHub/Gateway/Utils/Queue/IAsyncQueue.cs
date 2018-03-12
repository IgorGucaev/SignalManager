namespace EventsGateway.Gateway
{
    using EventsGateway.Common.Threading;

    //--//

    public interface IAsyncQueue<T>
    {
        void Push(T item);

        TaskWrapper<OperationStatus<T>> TryPop();

        int Count { get; }
    }
}
