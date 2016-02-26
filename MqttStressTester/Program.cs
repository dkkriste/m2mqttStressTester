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
                //var testSetup = new TestSetup(logger, "127.0.0.1", throughputTest, 128); // "40.118.3.244"
                //testSetup.RunTest();

                var broadcastTest = new BroadcastReceiveTest();
                var broadcastTestTestSetup = new TestSetup(logger, "13.80.15.211", broadcastTest, 1);
                broadcastTestTestSetup.RunTest(int.MaxValue, new TimeSpan(0, 30, 0), new TimeSpan(0, 0, 3), new TimeSpan(0, 0, 3), new TimeSpan(0, 0, 20), new TimeSpan(0, 0, 20));

                //var broadcastTest = new BroadcastTest();
                //var broadcastTestTestSetup = new TestSetup(logger, "40.115.17.8", broadcastTest, 16);
                //broadcastTestTestSetup.RunTest(int.MaxValue, new TimeSpan(0, 5, 0), new TimeSpan(0, 0, 3), new TimeSpan(0, 0, 3), new TimeSpan(0, 0, 10), new TimeSpan(0, 0, 10));

                //var concurrentConnectonTest = new ConcurrentConnectionTest();
                //var concurrentConnectonTestSetup = new TestSetup(logger, brokerIp, concurrentConnectonTest, 16);
                //concurrentConnectonTestSetup.RunTest(int.MaxValue, new TimeSpan(0, 1, 0), new TimeSpan(0, 0, 3), new TimeSpan(0, 0, 3), new TimeSpan(0, 0, 5),  new TimeSpan(0, 0, 10));

                Thread.Sleep(new TimeSpan(0, 0, 10));

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.WaitForFullGCComplete();
                GC.Collect();
            }
        }
    }
}
