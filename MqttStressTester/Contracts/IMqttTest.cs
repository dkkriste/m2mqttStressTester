namespace MqttStressTester.Tests
{
    using System;

    using MqttStressTester.Contracts;
    using MqttStressTester.Utils;

    public interface IMqttTest
    {
        IMqttTest Create(ILogger logger, string brokerIp, TestLimits testLimitses, ThreadSleepTimes threadSleepTimeses);

        void RunTest(int startupWaitMultiplier);
    }
}