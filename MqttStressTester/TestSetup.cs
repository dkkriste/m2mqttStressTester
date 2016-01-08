namespace MqttStressTester
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MqttStressTester.Contracts;
    using MqttStressTester.Tests;
    using MqttStressTester.Utils;

    public class TestSetup
    {
        public readonly int DefaultMaxNumberOfMessages;

        public readonly TimeSpan DefaultMaxTestTime;

        public readonly TimeSpan DefaultMinTimeBetweenMessages;

        public readonly TimeSpan DefaultMaxTimeBetweenMessages;

        private readonly ILogger logger;

        private readonly string brokerIp;

        private readonly IMqttTest testToBeRun;

        private readonly int threads;

        public TestSetup(ILogger logger, string brokerIp, IMqttTest testToBeRun, int threads)
        {
            this.logger = logger;
            this.brokerIp = brokerIp;
            this.testToBeRun = testToBeRun;
            this.threads = threads;

            DefaultMaxTestTime = new TimeSpan(0, 5, 0);
            DefaultMaxNumberOfMessages = int.MaxValue;
            DefaultMinTimeBetweenMessages = new TimeSpan(0, 0, 0, 0, 10);
            DefaultMaxTimeBetweenMessages = new TimeSpan(0, 0, 0, 0, 50);
        }

        /// <summary>
        /// Uses default values set in constructor
        /// </summary>
        public void RunThroughputTest()
        {
            var tasks = new List<Task>();
            for (var i = 0; i < threads; i++)
            {
                var timeLimits = CreateTestLimits(DefaultMaxNumberOfMessages, DefaultMaxTestTime);
                var threadSleepTimes = CreateThreadSleepTimes(DefaultMinTimeBetweenMessages, DefaultMaxTimeBetweenMessages);
                var test = testToBeRun.Create(logger, brokerIp, timeLimits, threadSleepTimes);
                tasks.Add(Task.Run(() => test.RunTest()));
            }

            foreach (var task in tasks)
            {
                task.Wait();
            }
        }

        public void RunThroughputTest(int maxNumberOfMessages, TimeSpan maxTestTime, TimeSpan minTimeBetweenMessages, TimeSpan maxTimeBetweenMessages)
        {
            var tasks = new List<Task>();
            for (var i = 0; i < threads; i++)
            {
                var timeLimits = CreateTestLimits(maxNumberOfMessages, maxTestTime);
                var threadSleepTimes = CreateThreadSleepTimes(minTimeBetweenMessages, maxTimeBetweenMessages);
                var test = testToBeRun.Create(logger, brokerIp, timeLimits, threadSleepTimes);
                tasks.Add(Task.Run(() => test.RunTest()));
            }

            foreach (var task in tasks)
            {
                task.Wait();
            }
        }

        private TestLimits CreateTestLimits(int maxNumberOfMessages, TimeSpan maxTestTime)
        {
            return new TestLimits(maxNumberOfMessages, maxTestTime);
        }

        private ThreadSleepTimes CreateThreadSleepTimes(TimeSpan minTimeBetweenMessages, TimeSpan maxTimeBetweenMessages)
        {
            return new ThreadSleepTimes(minTimeBetweenMessages, maxTimeBetweenMessages);
        }
    }
}