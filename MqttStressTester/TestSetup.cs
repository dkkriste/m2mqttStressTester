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

        public readonly TimeSpan DefaultMaxStartupDelay;

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

            DefaultMaxTestTime = new TimeSpan(0, 1, 0);
            DefaultMaxNumberOfMessages = int.MaxValue;
            DefaultMinTimeBetweenMessages = new TimeSpan(0, 0, 0, 0, 10);
            DefaultMaxTimeBetweenMessages = new TimeSpan(0, 0, 0, 0, 50);
            DefaultMaxStartupDelay = new TimeSpan(0, 0, 30);
        }

        /// <summary>
        /// Uses default values set in constructor
        /// </summary>
        public void RunTest()
        {
            var tasks = new List<Task>();
            for (var i = 0; i < threads; i++)
            {
                var timeLimits = CreateTestLimits(DefaultMaxNumberOfMessages, DefaultMaxTestTime);
                var threadSleepTimes = CreateThreadSleepTimes(DefaultMinTimeBetweenMessages, DefaultMaxTimeBetweenMessages, DefaultMaxStartupDelay);
                var test = testToBeRun.Create(logger, brokerIp, timeLimits, threadSleepTimes);
                tasks.Add(Task.Run(() => test.RunTest()));
            }

            foreach (var task in tasks)
            {
                try
                {
                    task.Wait();
                }
                catch (Exception exception)
                {
                    logger.LogException(exception);
                }
            }
        }

        public void RunTest(int maxNumberOfMessages, TimeSpan maxTestTime, TimeSpan minTimeBetweenMessages, TimeSpan maxTimeBetweenMessages, TimeSpan maxStartupDelay)
        {
            var tasks = new List<Task>();
            for (var i = 0; i < threads; i++)
            {
                var timeLimits = CreateTestLimits(maxNumberOfMessages, maxTestTime);
                var threadSleepTimes = CreateThreadSleepTimes(minTimeBetweenMessages, maxTimeBetweenMessages, maxStartupDelay);
                var test = testToBeRun.Create(logger, brokerIp, timeLimits, threadSleepTimes);
                try
                {
                    tasks.Add(Task.Run(() => test.RunTest()));
                }
                catch (Exception exception)
                {
                    logger.LogException(exception);
                }
            }

            foreach (var task in tasks)
            {
                try
                {
                    task.Wait();
                }
                catch (Exception exception)
                {
                    logger.LogException(exception);
                }
            }
        }

        private TestLimits CreateTestLimits(int maxNumberOfMessages, TimeSpan maxTestTime)
        {
            return new TestLimits(maxNumberOfMessages, maxTestTime);
        }

        private ThreadSleepTimes CreateThreadSleepTimes(TimeSpan minTimeBetweenMessages, TimeSpan maxTimeBetweenMessages, TimeSpan maxStartupDelay)
        {
            return new ThreadSleepTimes(minTimeBetweenMessages, maxTimeBetweenMessages, maxStartupDelay);
        }
    }
}