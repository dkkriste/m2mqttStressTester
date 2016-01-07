namespace MqttStressTester.Utils
{
    using System;

    public class TestLimits
    {
        public readonly int MaxNumberOfMessages;

        private readonly DateTimeOffset startTime;

        private readonly TimeSpan maxDuration;

        public TestLimits(int maxNumberOfMessages, TimeSpan maxDuration)
        {
            startTime = DateTimeOffset.Now;
            this.maxDuration = maxDuration;
            this.MaxNumberOfMessages = maxNumberOfMessages;
        }

        public int NumberOfMessagesSent { get; private set; }

        public int NumberOfMessagesRecieved { get; private set; }

        public bool AreMaxMessagesSendt()
        {
            return NumberOfMessagesSent >= MaxNumberOfMessages;
        }

        public bool IsTimeUp()
        {
            return DateTimeOffset.Now >= startTime + maxDuration;
        }

        public bool AreAllSendtMessagesRecieved()
        {
            return NumberOfMessagesSent <= NumberOfMessagesRecieved;
        }

        public void MessageRecieved()
        {
            NumberOfMessagesRecieved++;
        }

        public void MessagesSent()
        {
            NumberOfMessagesSent++;
        }

        public TimeSpan TimeSpendt()
        {
            return DateTimeOffset.Now - startTime;
        }
    }
}