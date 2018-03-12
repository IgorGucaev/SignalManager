using EventsStore.Test;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace EventsManager.LocalEventStorage.Test
{
    public class TestDataGenerator
    {
        static public IEnumerable<object[]> GetRandomAntennaSignalRow()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new object[]
                {
                    JsonConvert.SerializeObject(GetRandomAntennaData()),
                    DateTime.Now
                };
            }
        }

        static public AntennaData GetRandomAntennaData() => new AntennaData(Guid.NewGuid().ToString(), new Random().Next(-1000, 1000), DateTime.Now);
    }
}