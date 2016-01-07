﻿namespace MqttStressTester.Tests
{
    using System;
    using System.Text;
    using System.Threading;

    using MqttStressTester.Contracts;
    using MqttStressTester.Models;
    using MqttStressTester.Utils;

    using Newtonsoft.Json;

    using uPLibrary.Networking.M2Mqtt.Messages;

    public class ConcurrentConnectionTest : TestBase, IMqttTest
    {
        public ConcurrentConnectionTest() : base(null, string.Empty, null, null, string.Empty)
        {
        }

        public ConcurrentConnectionTest(ILogger logger, string brokerIp, TestLimits testLimits, ThreadSleepTimes threadSleepTimes)
            : base(logger, brokerIp, testLimits, threadSleepTimes, "ConcurrentConnectionTest")
        {
        }

        public IMqttTest Create(ILogger logger, string brokerIp, TestLimits testLimitses, ThreadSleepTimes threadSleepTimeses)
        {
            return new ConcurrentConnectionTest(logger, brokerIp, testLimitses, threadSleepTimeses);
        }

        public void RunTest()
        {
            this.LogTestBegin();

            this.ConnectMqtt();
            this.SubscribeMqtt(this.ClientId.ToString());

            while (!this.TestLimits.IsTimeUp())
            {
                this.LogMetric(LoggerConstants.IsAlive, 1);
                Thread.Sleep(this.ThreadSleepTimes.GetRandomSleepTime());
            }

            this.DisconectMqtt();
          
            this.LogTestEnd();
        }

        protected override void OnMqttClientMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            try
            {
                var serializedMessage = Encoding.UTF8.GetString(e.Message);
                this.LogEvent("Recieved", serializedMessage);
            }
            catch (Exception exception)
            {
                this.LogException(exception);
                this.LogEvent("Exception", this.Client.IsConnected.ToString());
            }
        }
    }
}