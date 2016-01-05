namespace MqttStressTester.Utils
{
    using System;

    public class Logger
    {
        public void LogMessage(string messageType, string message)
        {
            Console.WriteLine(messageType + ": " + message);
        }
    }
}