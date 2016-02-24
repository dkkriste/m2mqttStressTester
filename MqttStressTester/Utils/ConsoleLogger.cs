namespace MqttStressTester.Utils
{
    using System;

    using MqttStressTester.Contracts;

    public class ConsoleLogger : ILogger
    {
        public void LogException(Exception exception)
        {
            Console.WriteLine("Exception: " + exception.Message + " __ " + exception.StackTrace);
        }

        public void LogEvent(string eventName, string message)
        {
            Console.WriteLine(eventName + " : " + message);
        }

        public void LogMetric(string metricName, double value)
        {
            Console.WriteLine(metricName + " : " + value);
        }
    }
}