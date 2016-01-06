namespace MqttStressTester
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Azure;

    using MqttStressTester.Contracts;
    using MqttStressTester.Tests;
    using MqttStressTester.Utils;

    public class Program
    {
        public static void Main(string[] args)
        {
            ILogger logger = new ApplicationInsightsLogger();
            var brokerIp = CloudConfigurationManager.GetSetting("BrokerIp");

            var maxTestTime = new TimeSpan(0, 1, 0);
            var maxNumberOfMessages = 10000;

            var minTimeBetweenMessages = new TimeSpan(0, 0, 0, 0, 10);
            var maxTimeBetweenMessages = new TimeSpan(0, 0, 0, 0, 100);

            var tasks = new List<Task>();

            for (int i = 0; i < 16; i++)
            {
                var throughputTest = new MessageThroughputTest(logger);
                tasks.Add(Task.Run(() => throughputTest.Test(brokerIp, maxTestTime, maxNumberOfMessages, minTimeBetweenMessages, maxTimeBetweenMessages)));
            }

            foreach (var task in tasks)
            {
                task.Wait();
            }
        }
    }
}
