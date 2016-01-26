namespace MqttStressTester
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
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
            //concurrentConnectonTestSetup.RunTest(int.MaxValue, new TimeSpan(0, 10, 0), new TimeSpan(0, 1, 0), new TimeSpan(0, 1, 0));

            while (true)
            {
                //var throughputTest = new MessageThroughputTest();
                //var testSetup = new TestSetup(logger, brokerIp, throughputTest, 32); // "40.118.3.244"
                //testSetup.RunTest();

                var concurrentConnectonTest = new ConcurrentConnectionTest();
                var concurrentConnectonTestSetup = new TestSetup(logger, brokerIp, concurrentConnectonTest, 16);
                concurrentConnectonTestSetup.RunTest(int.MaxValue, new TimeSpan(0, 1, 0), new TimeSpan(0, 0, 3), new TimeSpan(0, 0, 3), new TimeSpan(0, 0, 5),  new TimeSpan(0, 0, 10));
            }
        }
    }
}
