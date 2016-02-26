namespace MqttStressTester.Utils
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    public class TestLimits
    {
        public readonly int MaxNumberOfMessages;

        private readonly DateTimeOffset startTime;

        private readonly TimeSpan maxDuration;

        private readonly Stopwatch stopwatch;

        public TestLimits(int maxNumberOfMessages, TimeSpan maxDuration)
        {
            stopwatch = new Stopwatch();
            startTime = DateTimeOffset.Now;
            this.maxDuration = maxDuration;
            this.MaxNumberOfMessages = maxNumberOfMessages;
        }

        public int NumberOfMessagesSent { get; private set; }

        public int NumberOfMessagesRecieved; 

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
            Interlocked.Increment(ref NumberOfMessagesRecieved);
        }

        public void MessagesSent()
        {
            NumberOfMessagesSent++;
        }

        public TimeSpan TimeSpendt()
        {
            return DateTimeOffset.Now - startTime;
        }

        public void StartTest()
        {
            stopwatch.Start();
        }

        public void EndTest()
        {
            stopwatch.Stop();
        }

        public TimeSpan ActualTestTime()
        {
            return stopwatch.Elapsed;
        }
    }
}