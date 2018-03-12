using System;

namespace EventsStore.Test
{
    public class AntennaData
    {
        public string epc { get; private set; }
        public decimal value { get; private set; }
        public DateTime date { get; private set; }

        public AntennaData(string epc, decimal value, DateTime date)
        {
            this.epc = epc;
            this.value = value;
            this.date = date;
        }
    }
}