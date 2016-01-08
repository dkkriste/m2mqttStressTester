namespace MqttStressTester.Utils
{
    using System;

    public static class TimeSpanMillisecondExtention
    {
        public static double GetMilliseconds(this TimeSpan timespan)
        {
            return timespan.Ticks / 10000.0;
        }
    }
}