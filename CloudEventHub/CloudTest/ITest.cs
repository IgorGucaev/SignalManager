using System;
using System.Collections.Generic;
using System.Text;

namespace EventsGateway.Test
{
    interface ITest
    {
        void Run();
        void Completed();
        int TotalMessagesSent { get; }
        int TotalMessagesToSend { get; }
    }
}
