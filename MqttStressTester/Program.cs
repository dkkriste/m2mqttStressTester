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
            var throughputTest = new MessageThroughputTest();

            var testSetup = new TestSetup(logger, brokerIp, throughputTest, 1);
            testSetup.RunThroughputTest();
        }
    }
}
