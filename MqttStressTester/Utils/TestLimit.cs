namespace MqttStressTester.Utils
{
    using System;

    public class TestLimit
    {
        private readonly DateTimeOffset startTime;

        private readonly TimeSpan maxDuration;

        private readonly int maxNumberOfMessages;

        public TestLimit(TimeSpan maxDuration, int maxNumberOfMessages)
        {
            startTime = DateTimeOffset.Now;
            this.maxDuration = maxDuration;
            this.maxNumberOfMessages = maxNumberOfMessages;
        }

        public int NumberOfMessagesSent { get; private set; }

        public int NumberOfMessagesRecieved { get; private set; }

        public bool IsTestComplete()
        {
            if (NumberOfMessagesSent >= maxNumberOfMessages || DateTimeOffset.Now >= startTime + maxDuration)
            {
                return true;
            }

            return false;
        }

        public void MessageRecieved()
        {
            NumberOfMessagesRecieved++;
        }

        public void MessagesSent()
        {
            NumberOfMessagesSent++;
        }
    }
}