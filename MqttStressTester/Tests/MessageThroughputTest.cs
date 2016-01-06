namespace MqttStressTester.Tests
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using MqttStressTester.Contracts;
    using MqttStressTester.Models;
    using MqttStressTester.Utils;

    using Newtonsoft.Json;

    using uPLibrary.Networking.M2Mqtt;
    using uPLibrary.Networking.M2Mqtt.Messages;

    public class MessageThroughputTest : IMqttTest
    {
        private readonly ILogger logger;

        private MqttClient client;

        private Guid clientId;

        private TestLimit testLimit;

        private ThreadSleepTime threadSleepTime;

        private ThroughputTimeMessage[] messages;

        private TimeSpan totalTime;

        private TimeSpan maxTime;

        public MessageThroughputTest(ILogger logger)
        {
            this.logger = logger;
        }

        public void Test(string brokerIp, TimeSpan maxDuration, int maxNumberOfMessages, TimeSpan minTimeBetweenMessages, TimeSpan maxTimeBetweenMessages)
        {
            InitConnection(brokerIp);
            Init(maxDuration, maxNumberOfMessages, minTimeBetweenMessages, maxTimeBetweenMessages);

            logger.LogMetric("Start", 1);

            while (!testLimit.AreMaxMessagesSendt() && !testLimit.IsTimeUp())
            {
                var message = new ThroughputTimeMessage { MessageNumber = testLimit.NumberOfMessagesSent, MessageSendtTime = DateTimeOffset.Now };
                messages[message.MessageNumber] = message;

                var serializedMessage = JsonConvert.SerializeObject(message);
                Publish(clientId.ToString(), serializedMessage);
                testLimit.MessagesSent();

                Thread.Sleep(threadSleepTime.GetRandomSleepTime());
            }

            while (!testLimit.AreAllSendtMessagesRecieved() && !testLimit.IsTimeUp())
            {
                Thread.Sleep(threadSleepTime.GetRandomSleepTime());
            }

            client.Disconnect();
            logger.LogMetric("Max", maxTime.Ticks);
            logger.LogMetric("Average", totalTime.Ticks / (testLimit.NumberOfMessagesRecieved * 1.0));
            logger.LogMetric("End", 1);
        }

        private void Init(TimeSpan maxDuration, int maxNumberOfMessages, TimeSpan minTimeBetweenMessages, TimeSpan maxTimeBetweenMessages)
        {
            testLimit = new TestLimit(maxDuration, maxNumberOfMessages);
            threadSleepTime = new ThreadSleepTime(minTimeBetweenMessages, maxTimeBetweenMessages);
            messages = new ThroughputTimeMessage[maxNumberOfMessages];
            totalTime = new TimeSpan();
            maxTime = new TimeSpan();
        }

        private void InitConnection(string brokerIp)
        {
            clientId = Guid.NewGuid();

            client = new MqttClient(brokerIp);
            client.MqttMsgPublishReceived += OnMqttClientMsgPublishReceived;

            client.Connect(clientId.ToString());
            Subscribe(clientId.ToString());
        }

        private void Subscribe(string topicName)
        {
            client.Subscribe(new[] { topicName }, new[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
        }

        private void Publish(string topic, string message)
        {
            client.Publish(topic, Encoding.UTF8.GetBytes(message), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
        }

        private void OnMqttClientMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            try
            {
                var serializedMessage = Encoding.UTF8.GetString(e.Message);
                var message = JsonConvert.DeserializeObject<ThroughputTimeMessage>(serializedMessage);
                var roundTripTime = DateTimeOffset.Now - messages[message.MessageNumber].MessageSendtTime;
                totalTime += roundTripTime;
                if (roundTripTime > maxTime)
                {
                    maxTime = roundTripTime;
                }

                testLimit.MessageRecieved();
            }
            catch (Exception exception)
            {
                logger.LogException(exception);
            }
        }
    }
}