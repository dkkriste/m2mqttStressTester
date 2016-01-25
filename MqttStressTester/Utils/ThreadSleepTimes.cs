namespace MqttStressTester.Utils
{
    using System;

    public class ThreadSleepTimes
    {
        private readonly TimeSpan minSleepTime;

        private readonly TimeSpan maxSleepTime;

        private readonly TimeSpan fixedStartupDelay;

        private readonly TimeSpan maxRandomStartupDelay;

        private readonly Random randomNumberGenerator;

        public ThreadSleepTimes(TimeSpan minSleepTime, TimeSpan maxSleepTime, TimeSpan fixedStartupDelay, TimeSpan maxRandomStartupDelay)
        {
            this.minSleepTime = minSleepTime;
            this.maxSleepTime = maxSleepTime;
            this.fixedStartupDelay = fixedStartupDelay;
            this.maxRandomStartupDelay = maxRandomStartupDelay;
            randomNumberGenerator = new Random();
        }

        public TimeSpan GetFixedStartupTime(int startupWaitMultiplier)
        {
            return new TimeSpan(this.fixedStartupDelay.Ticks * startupWaitMultiplier);
        }

        public TimeSpan GetRandomStartupTime()
        {
            var randomNumberInInterval = LongRandom(0, maxRandomStartupDelay.Ticks);
            return new TimeSpan(randomNumberInInterval);
        }

        public TimeSpan GetRandomSleepTime()
        {
            var minTicks = minSleepTime.Ticks;
            var maxTicks = maxSleepTime.Ticks;

            if (minTicks >= maxTicks)
            {
                return new TimeSpan(minTicks);
            }

            var randomNumberInInterval = LongRandom(minTicks, maxTicks);

            return new TimeSpan(randomNumberInInterval);
        }

        public TimeSpan GetAverageSleepTime()
        {
            var totalTicks = minSleepTime.Ticks + maxSleepTime.Ticks;
            return new TimeSpan(totalTicks / 2);
        }

        private long LongRandom(long min, long max)
        {
            byte[] buf = new byte[8];
            randomNumberGenerator.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);

            return Math.Abs(longRand % (max - min)) + min;
        }
    }
}