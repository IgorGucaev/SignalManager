using EventsManager.Abstractions;
using EventsManager.LocalEventStorage.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace EventsManager.Test
{
    public class EventManagerTest : BaseTest
    {
        [Fact]
        public void Test_EventManager()
        {
            // Prepare
            IEventManager manager = this.GetService<IEventManager>();
            manager.Start();
            ISignalService signals = this.GetService<ISignalService>();
            signals.Truncate();
            int countBefore = signals.Count();


            // Pre-validate

            // Perform
            List<string> data = new List<string>();
            for (int i = 0; i < 100; i++)
                data.Add("{ guid: \"" + Guid.NewGuid().ToString() + "\"}");

            foreach (string signal in data)
            {
                manager.RegisterEvent(Signal.New("fooDevice", signal, DateTime.Now));
                Thread.Sleep(5);
            }
            Thread.Sleep(60 * 1000); // Wait while manager save data to the end

            // Post-validate
            int countAfter = signals.Count();
            Assert.Equal(0, countAfter);
            manager.Stop();
        }
    }
}