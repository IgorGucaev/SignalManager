namespace EventsGateway.Gateway
{
    using System;
    using System.Collections.Concurrent;
    using EventsGateway.Common;
    using EventsGateway.Common.Threading;

    //--//

    public class GatewayQueue<T> : IAsyncQueue<T>
    {
        private readonly ConcurrentQueue<T> _Queue = new ConcurrentQueue<T>( );

        //--//

        async public void Push( T item )
        {
            _Queue.Enqueue( item );
        }

        public TaskWrapper<OperationStatus<T>> TryPop( )
        {
            Func<OperationStatus<T>> deque = ( ) =>
            {
                T returnedItem;

                bool isReturned = _Queue.TryDequeue( out returnedItem );

                if( isReturned )
                {
                    return OperationStatusFactory.CreateSuccess<T>( returnedItem );
                }

                return OperationStatusFactory.CreateError<T>( ErrorCode.NoDataReceived );
            };

            var sf = new SafeFunc<OperationStatus<T>>( deque, null );

            return TaskWrapper<OperationStatus<T>>.Run( () => sf.SafeInvoke() );
        }

        public int Count
        {
            get
            {
                return _Queue.Count;
            }
        }
    }
}
