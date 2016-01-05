namespace MqttStressTester
{
    using System;

    using MqttStressTester.Tests;

    public class Program
    {
        public static void Main(string[] args)
        {
            var maxTestTime = new TimeSpan(0, 0, 1);
            var maxNumberOfMessages = 20;

            var minTimeBetweenMessages = new TimeSpan(0, 0, 0, 0, 1);
            var maxTimeBetweenMessages = new TimeSpan(0, 0, 0, 0, 10);

            var throughputTest = new MessageThroughputTest();
            throughputTest.Test("", maxTestTime, maxNumberOfMessages, minTimeBetweenMessages, maxTimeBetweenMessages);
        }
    }
}
