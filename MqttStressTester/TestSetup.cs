namespace MqttStressTester
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
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

        public readonly TimeSpan DefaultFixedStartupDelay;

        public readonly TimeSpan DefaultMaxStartupDelay;

        private readonly ILogger logger;

        private readonly string brokerIp;

        private readonly IMqttTest testToBeRun;

        private readonly int numberOfThreads;

        public TestSetup(ILogger logger, string brokerIp, IMqttTest testToBeRun, int numberOfThreads)
        {
            this.logger = logger;
            this.brokerIp = brokerIp;
            this.testToBeRun = testToBeRun;
            this.numberOfThreads = numberOfThreads;

            DefaultMaxTestTime = new TimeSpan(0, 1, 0);
            DefaultMaxNumberOfMessages = int.MaxValue;
            DefaultMinTimeBetweenMessages = new TimeSpan(0, 0, 0, 0, 10);
            DefaultMaxTimeBetweenMessages = new TimeSpan(0, 0, 0, 0, 50);
            DefaultFixedStartupDelay = new TimeSpan(0, 0, 10);
            DefaultMaxStartupDelay = new TimeSpan(0, 0, 10);
        }

        /// <summary>
        /// Uses default values set in constructor
        /// </summary>
        public void RunTest()
        {
            var tasks = new List<Task>();
            for (var i = 0; i < numberOfThreads; i++)
            {
                var timeLimits = CreateTestLimits(DefaultMaxNumberOfMessages, DefaultMaxTestTime);
                var threadSleepTimes = CreateThreadSleepTimes(DefaultMinTimeBetweenMessages, DefaultMaxTimeBetweenMessages, DefaultFixedStartupDelay, DefaultMaxStartupDelay);
                var test = testToBeRun.Create(logger, brokerIp, timeLimits, threadSleepTimes);
                var startupWaitMultiplier = i / 10;
                var task = Task.Factory.StartNew(() => test.RunTest(startupWaitMultiplier), TaskCreationOptions.LongRunning);
                tasks.Add(task);
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

        public void RunTest(int maxNumberOfMessages, TimeSpan maxTestTime, TimeSpan minTimeBetweenMessages, TimeSpan maxTimeBetweenMessages, TimeSpan fixedStartupDelay, TimeSpan maxStartupDelay)
        {
            var tasks = new List<Task>();
            for (var i = 0; i < numberOfThreads; i++)
            {
                var timeLimits = CreateTestLimits(maxNumberOfMessages, maxTestTime);
                var threadSleepTimes = CreateThreadSleepTimes(minTimeBetweenMessages, maxTimeBetweenMessages, fixedStartupDelay, maxStartupDelay);
                var test = testToBeRun.Create(logger, brokerIp, timeLimits, threadSleepTimes);
                var startupWaitMultiplier = i / 10;
                Task task = Task.Factory.StartNew(() => test.RunTest(startupWaitMultiplier), TaskCreationOptions.LongRunning);
                tasks.Add(task);
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

        private ThreadSleepTimes CreateThreadSleepTimes(TimeSpan minTimeBetweenMessages, TimeSpan maxTimeBetweenMessages, TimeSpan fixedStartupDelay, TimeSpan maxStartupDelay)
        {
            return new ThreadSleepTimes(minTimeBetweenMessages, maxTimeBetweenMessages, fixedStartupDelay, maxStartupDelay);
        }
    }
}