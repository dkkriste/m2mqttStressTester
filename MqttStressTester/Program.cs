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

            var concurrentConnectonTest = new ConcurrentConnectionTest();
            var concurrentConnectonTestSetup = new TestSetup(logger, brokerIp, concurrentConnectonTest, 2);
            concurrentConnectonTestSetup.RunThroughputTest(int.MaxValue, new TimeSpan(0, 10, 0), new TimeSpan(0, 1, 0), new TimeSpan(0, 1, 0));

            //var throughputTest = new MessageThroughputTest();
            //var throughputTestSetup = new TestSetup(logger, brokerIp, throughputTest, 2);
            //throughputTestSetup.RunThroughputTest();
        }
    }
}
