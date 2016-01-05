namespace MqttStressTester.Tests
{
    using System;
    using System.Text;
    using System.Threading;

    using MqttStressTester.Models;
    using MqttStressTester.Utils;

    using Newtonsoft.Json;

    using uPLibrary.Networking.M2Mqtt;
    using uPLibrary.Networking.M2Mqtt.Messages;

    public class MessageThroughputTest : IMqttTest
    {
        private readonly Logger logger;

        private MqttClient client;

        private Guid clientId;

        private TestLimit testLimit;

        private ThreadSleepTime threadSleepTime;

        private ThroughputTimeMessage[] messages;

        public MessageThroughputTest()
        {
            logger = new Logger();
        }

        public void Test(string brokerIp, TimeSpan maxDuration, int maxNumberOfMessages, TimeSpan minTimeBetweenMessages, TimeSpan maxTimeBetweenMessages)
        {
            InitConnection(brokerIp);
            testLimit = new TestLimit(maxDuration, maxNumberOfMessages);
            threadSleepTime = new ThreadSleepTime(minTimeBetweenMessages, maxTimeBetweenMessages);
            messages = new ThroughputTimeMessage[maxNumberOfMessages];

            while (!testLimit.IsTestComplete())
            {
                var message = new ThroughputTimeMessage { MessageNumber = testLimit.NumberOfMessagesSent, MessageSendtTime = DateTimeOffset.Now };
                messages[message.MessageNumber] = message;

                var serializedMessage = JsonConvert.SerializeObject(message);
                Publish(clientId.ToString(), serializedMessage);
                testLimit.MessagesSent();

                Thread.Sleep(threadSleepTime.GetRandomSleepTime());
            }
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
                var roundTripTime = messages[message.MessageNumber].MessageSendtTime - DateTimeOffset.Now;
                logger.LogMessage("MessageThroughputTest", roundTripTime.ToString());
                testLimit.MessageRecieved();
            }
            catch (Exception exception)
            {
                logger.LogMessage("Exception", exception.Message);
            }
        }
    }
}