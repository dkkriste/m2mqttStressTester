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

    public class BroadcastTest : TestBase, IMqttTest
    {
        private TimeSpan totalTime;

        private TimeSpan maxTime;

        private TimeSpan publishTime;

        public BroadcastTest() : base(null, string.Empty, null, null, string.Empty)
        {
        }

        public BroadcastTest(ILogger logger, string brokerIp, TestLimits testLimits, ThreadSleepTimes threadSleepTimes)
            : base(logger, brokerIp, testLimits, threadSleepTimes, "BroadcastTest")
        {
            totalTime = new TimeSpan();
            maxTime = new TimeSpan();
            publishTime = new TimeSpan();
        }

        public IMqttTest Create(ILogger logger, string brokerIp, TestLimits testLimitses, ThreadSleepTimes threadSleepTimeses)
        {
            return new BroadcastTest(logger, brokerIp, testLimitses, threadSleepTimeses);
        }

        public void RunTest(int startupWaitMultiplier)
        {
            var startupWaitTime = this.ThreadSleepTimes.GetFixedStartupTime(startupWaitMultiplier);

            Thread.Sleep(startupWaitTime);
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            try
            {
                this.ConnectMqtt();

                stopWatch.Stop();
                this.LogMetric(LoggerConstants.ConnectAndSubscribe, stopWatch.Elapsed.GetMilliseconds());
                stopWatch.Reset();
            }
            catch (Exception exception)
            {
                this.LogException(exception);
                return;
            }

            this.TestLimits.StartTest();
            while (!this.TestLimits.IsTimeUp())
            {
                stopWatch.Start();
                var message = new ThroughputTimeMessage { MessageNumber = this.TestLimits.NumberOfMessagesSent, MessageSendtTime = DateTimeOffset.Now };
                var serializedMessage = JsonConvert.SerializeObject(message);
                this.PublishMqtt("ServerUserOnlineTopic", serializedMessage);
                this.TestLimits.MessagesSent();

                stopWatch.Stop();

                publishTime += stopWatch.Elapsed;
                var sleepTime = this.ThreadSleepTimes.GetRandomSleepTime();
                if (stopWatch.Elapsed < sleepTime)
                {
                    Thread.Sleep(sleepTime - stopWatch.Elapsed);
                }

                stopWatch.Reset();
            }

            this.TestLimits.EndTest();

            try
            {
                this.DisconectMqtt();
            }
            catch (Exception exception)
            {
                this.LogException(exception);
            }

            this.LogMetric(LoggerConstants.PublishTime, publishTime.GetMilliseconds() / this.TestLimits.NumberOfMessagesSent);
            this.LogMetric(LoggerConstants.Sent, this.TestLimits.NumberOfMessagesSent);
        }

        protected override void OnMqttClientMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            try
            {
                var serializedMessage = Encoding.UTF8.GetString(e.Message);
                var message = JsonConvert.DeserializeObject<ThroughputTimeMessage>(serializedMessage);
                var roundTripTime = DateTimeOffset.Now - message.MessageSendtTime;
                totalTime += roundTripTime;
                if (roundTripTime > maxTime)
                {
                    maxTime = roundTripTime;
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