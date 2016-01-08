namespace MqttStressTester.Utils
{
    using System;

    public class ThreadSleepTimes
    {
        private readonly TimeSpan minSleepTime;

        private readonly TimeSpan maxSleepTime;

        private readonly Random randomNumberGenerator;

        public ThreadSleepTimes(TimeSpan minSleepTime, TimeSpan maxSleepTime)
        {
            this.minSleepTime = minSleepTime;
            this.maxSleepTime = maxSleepTime;
            randomNumberGenerator = new Random();
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