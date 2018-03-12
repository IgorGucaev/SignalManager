﻿
namespace EventsGateway.Test
{
    using System;
    using System.Threading;
    using EventsGateway.Gateway;
    using EventsGateway.Common.Threading;

    //--//

    internal class MockSender<T> : IMessageSender<T>
    {
        private static readonly int MAX_LAG = 1; // ms

        //--//

        protected readonly ITest _test;
        protected readonly Random _rand;
        protected int _forSending;

        //--//

        internal MockSender(ITest test)
        {
            _forSending = 0;
            _test = test;
            _rand = new Random();
        }

        public TaskWrapper SendMessage(T data)
        {
            SimulateSend();
            return null;
        }

        public TaskWrapper SendSerialized(string jsonData)
        {
            SimulateSend();
            return null;
        }

        public void Close()
        {
        }

        private void SimulateSend()
        {
            // Naive atetmpt to simulate network latency
            Thread.Sleep(_rand.Next(MAX_LAG));

            int totalMessagesSent = _test.TotalMessagesSent;

            // LORENZO: print all data and validate that they match the data sent
            if (Interlocked.Increment(ref _forSending) == totalMessagesSent && totalMessagesSent >= _test.TotalMessagesToSend)
            {
                _test.Completed();
            }
        }
    }
}