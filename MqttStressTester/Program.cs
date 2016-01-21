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
            ILogger logger = new ConsoleLogger();
            var brokerIp = CloudConfigurationManager.GetSetting("BrokerIp");

            //var concurrentConnectonTest = new ConcurrentConnectionTest();
            //var concurrentConnectonTestSetup = new TestSetup(logger, brokerIp, concurrentConnectonTest, 1);
            //concurrentConnectonTestSetup.RunThroughputTest(int.MaxValue, new TimeSpan(0, 10, 0), new TimeSpan(0, 1, 0), new TimeSpan(0, 1, 0));

            while (true)
            {
                var throughputTest = new MessageThroughputTest();
                var throughputTestSetup = new TestSetup(logger, brokerIp, throughputTest, 16);
                throughputTestSetup.RunThroughputTest(int.MaxValue, new TimeSpan(0, 1, 0), new TimeSpan(0, 0, 0, 0, 10), new TimeSpan(0, 0, 0, 0, 50));
            }
        }
    }
}
