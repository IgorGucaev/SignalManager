using EventsManager.LocalEventStorage.Abstractions;
using EventsManager.LocalEventStorage.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace EventsManager.LocalEventStorage.Test
{
    public class ServiceTest : BaseTest
    {
        [Fact]
        public void Test_Service_AddScope()
        {
            // Prepare
            ISignalService signals = this.GetService<ISignalService>();
            int countBefore = signals.Count();

            // Pre-validate

            // Perform
            List<Signal> newSignals = new List<Signal>()
            {
                Signal.New("fooDevice", "foo", DateTime.Now),
                Signal.New("barDevice", "bar", DateTime.Now.AddSeconds(1)),
                Signal.New("bazDevice", "baz", DateTime.Now.AddSeconds(2))
            };
            signals.Add(newSignals);

            // Post-validate
            int countAfter = signals.Count();
            Assert.Equal(countBefore + newSignals.Count, countAfter);
        }

        [Theory]
        [InlineData(1000)]
        [InlineData(10000)]
        [InlineData(100000)]
        public void Test_Service_Truncate(int rowCount)
        {
            // Prepare
            ISignalService signals = this.GetService<ISignalService>();
            int countBefore = signals.Count();

            List<Signal> scope = new List<Signal>();
            for (int i = 0; i < rowCount; i++)
                scope.Add(Signal.New("fooDevice", "foo", DateTime.Now));

            signals.Add(scope);

            // Pre-validate
            Assert.Equal(rowCount + countBefore, signals.Count());

            // Perform
            signals.Truncate();

            // Post-validate
            Assert.Equal(0, signals.Count());
        }
    }
}
