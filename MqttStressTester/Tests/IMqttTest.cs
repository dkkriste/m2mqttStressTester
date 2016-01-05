namespace MqttStressTester.Tests
{
    using System;

    public interface IMqttTest
    {
        void Test(string brokerIp, TimeSpan maxDuration, int maxNumberOfMessages, TimeSpan minTimeBetweenMessages, TimeSpan maxTimeBetweenMessages);
    }
}