namespace MqttStressTester.Tests
{
    using System;
    using System.Text;

    using MqttStressTester.Contracts;
    using MqttStressTester.Utils;

    using uPLibrary.Networking.M2Mqtt;
    using uPLibrary.Networking.M2Mqtt.Messages;

    public abstract class TestBase
    {
        protected readonly string BrokerIp;

        protected readonly TestLimits TestLimits;

        protected readonly ThreadSleepTimes ThreadSleepTimes;

        protected readonly Guid ClientId;

        protected readonly MqttClient Client;

        private readonly ILogger logger;

        private readonly string testName;

        protected TestBase(ILogger logger, string brokerIp, TestLimits testLimits, ThreadSleepTimes threadSleepTimes, string testName)
        {
            this.logger = logger;
            this.BrokerIp = brokerIp;
            this.testName = testName;
            this.TestLimits = testLimits;
            this.ThreadSleepTimes = threadSleepTimes;

            ClientId = Guid.NewGuid();
            this.Client = new MqttClient(this.BrokerIp);
            this.Client.MqttMsgPublishReceived += OnMqttClientMsgPublishReceived;
        }

        #region MQTT

        protected void ConnectMqtt()
        {
            this.Client.Connect(ClientId.ToString());
        }

        protected void DisconectMqtt()
        {
            this.Client.MqttMsgPublishReceived -= OnMqttClientMsgPublishReceived;
            this.Client.Disconnect();
        }

        protected void SubscribeMqtt(string topicName)
        {
            this.Client.Subscribe(new[] { topicName }, new[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
        }

        protected void PublishMqtt(string topic, string message)
        {
            this.Client.Publish(topic, Encoding.UTF8.GetBytes(message), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
        }

        protected abstract void OnMqttClientMsgPublishReceived(object sender, MqttMsgPublishEventArgs e);

        #endregion

        #region Logging

        protected void LogTestBegin()
        {
            this.logger.LogMetric(testName + "_" + LoggerConstants.Running, 1);
        }

        protected void LogTestEnd()
        {
            this.logger.LogMetric(testName + "_" + LoggerConstants.Running, -1);
        }

        protected void LogMetric(string metricName, double value)
        {
            this.logger.LogMetric(testName + "_" + metricName, value);
        }

        protected void LogException(Exception exception)
        {
            this.logger.LogException(exception);
        }

        protected void LogEvent(string eventName, string message)
        {
            this.logger.LogEvent(testName + "_" + eventName, message);
        }

        #endregion
    }
}