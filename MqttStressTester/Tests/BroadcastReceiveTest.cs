namespace MqttStressTester.Tests
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using System.Threading;

    using MqttStressTester.Contracts;
    using MqttStressTester.Models;
    using MqttStressTester.Utils;

    using Newtonsoft.Json;

    using uPLibrary.Networking.M2Mqtt.Messages;

    public class BroadcastReceiveTest : TestBase, IMqttTest
    {
        private long totalTimeTicks;

        private long maxTimeTicks;

        public BroadcastReceiveTest() : base(null, string.Empty, null, null, string.Empty)
        {
        }

        public BroadcastReceiveTest(ILogger logger, string brokerIp, TestLimits testLimits, ThreadSleepTimes threadSleepTimes)
            : base(logger, brokerIp, testLimits, threadSleepTimes, "BroadcastReceiveTest")
        {
        }

        public IMqttTest Create(ILogger logger, string brokerIp, TestLimits testLimitses, ThreadSleepTimes threadSleepTimeses)
        {
            return new BroadcastReceiveTest(logger, brokerIp, testLimitses, threadSleepTimeses);
        }

        public void RunTest(int startupWaitMultiplier)
        {
            try
            {
                this.ConnectMqtt();
                this.SubscribeMqtt("ServerUserOnlineTopicForward");
                this.SubscribeMqtt("ServerUserOnlineTopic");
            }
            catch (Exception exception)
            {
                this.LogException(exception);
                return;
            }

            while (!this.TestLimits.IsTimeUp())
            {
                Thread.Sleep(this.ThreadSleepTimes.GetRandomSleepTime());
            }

            try
            {
                this.DisconectMqtt();
            }
            catch (Exception exception)
            {
                this.LogException(exception);
            }

            this.LogMetric(LoggerConstants.Max, maxTimeTicks / 10000);
            this.LogMetric(LoggerConstants.Received, this.TestLimits.NumberOfMessagesRecieved);
            if (this.TestLimits.NumberOfMessagesRecieved > 0)
            {
                this.LogMetric(LoggerConstants.Average, totalTimeTicks / 10000 / this.TestLimits.NumberOfMessagesRecieved);
                this.LogMetric(LoggerConstants.MessagesPrSecond, this.TestLimits.NumberOfMessagesRecieved / this.TestLimits.ActualTestTime().TotalSeconds);
            }
        }

        protected override void OnMqttClientMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            try
            {
                var serializedMessage = Encoding.UTF8.GetString(e.Message);
                var message = JsonConvert.DeserializeObject<ThroughputTimeMessage>(serializedMessage);
                var roundTripTime = (DateTimeOffset.Now - message.MessageSendtTime).Ticks;
                Interlocked.Add(ref totalTimeTicks, roundTripTime);
                if (roundTripTime > maxTimeTicks)
                {
                    maxTimeTicks = roundTripTime;
                }

                this.TestLimits.MessageRecieved();
            }
            catch (Exception exception)
            {
                this.LogException(exception);
            }
        }
    }
}