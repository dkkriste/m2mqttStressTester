namespace MqttStressTester.Tests
{
    using System;
    using System.Text;
    using System.Threading;

    using MqttStressTester.Contracts;
    using MqttStressTester.Models;
    using MqttStressTester.Utils;

    using Newtonsoft.Json;

    using uPLibrary.Networking.M2Mqtt.Messages;

    public class MessageThroughputTest : TestBase, IMqttTest
    {
        private TimeSpan totalTime;

        private TimeSpan maxTime;

        public MessageThroughputTest() : base(null, string.Empty, null, null, string.Empty)
        {
        }

        public MessageThroughputTest(
            ILogger logger,
            string brokerIp,
            TestLimits testLimitses,
            ThreadSleepTimes threadSleepTimeses) : base(logger, brokerIp, testLimitses, threadSleepTimeses, "ThroughputTest")
        {
            totalTime = new TimeSpan();
            maxTime = new TimeSpan();
        }

        public IMqttTest Create(
            ILogger logger,
            string brokerIp,
            TestLimits testLimitses,
            ThreadSleepTimes threadSleepTimeses)
        {
            return new MessageThroughputTest(logger, brokerIp, testLimitses, threadSleepTimeses);
        }

        public void RunTest()
        {
            this.LogTestBegin();

            this.ConnectMqtt();
            this.SubscribeMqtt(this.ClientId.ToString());

            while (!this.TestLimits.AreMaxMessagesSendt() && !this.TestLimits.IsTimeUp())
            {
                var message = new ThroughputTimeMessage { MessageNumber = this.TestLimits.NumberOfMessagesSent, MessageSendtTime = DateTimeOffset.Now };
                var serializedMessage = JsonConvert.SerializeObject(message);
                this.PublishMqtt(this.ClientId.ToString(), serializedMessage);
                this.TestLimits.MessagesSent();

                Thread.Sleep(this.ThreadSleepTimes.GetRandomSleepTime());
            }

            this.LogMetric(LoggerConstants.MissingMessages, this.TestLimits.NumberOfMessagesSent - this.TestLimits.NumberOfMessagesRecieved);

            while (!this.TestLimits.AreAllSendtMessagesRecieved() && !this.TestLimits.IsTimeUp())
            {
                Thread.Sleep(this.ThreadSleepTimes.GetRandomSleepTime());
            }

            this.DisconectMqtt();
             
            this.LogMetric(LoggerConstants.Max, maxTime.GetMilliseconds());
            this.LogMetric(LoggerConstants.Average, totalTime.GetMilliseconds() / this.TestLimits.NumberOfMessagesRecieved);
            this.LogMetric(LoggerConstants.Sent, this.TestLimits.NumberOfMessagesSent);
            this.LogMetric(LoggerConstants.MessagesPrSecond, this.TestLimits.NumberOfMessagesSent / (this.TestLimits.TimeSpendt().GetMilliseconds() / 1000));
            this.LogTestEnd();
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
                this.LogEvent(LoggerConstants.Exception, this.Client.IsConnected.ToString());
            }
        }
    }
}