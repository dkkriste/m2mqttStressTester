namespace ConcurrentConnectionStressTester
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Azure;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.ServiceRuntime;

    using MqttStressTester;
    using MqttStressTester.Contracts;
    using MqttStressTester.Tests;
    using MqttStressTester.Utils;

    public class WorkerRole : RoleEntryPoint
    {
        private const int DefaultNumberOfThreads = 8;

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("CloudMqttStressTester is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 16;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("CloudMqttStressTester has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("CloudMqttStressTester is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("CloudMqttStressTester has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            ILogger logger = new ApplicationInsightsLogger();
            string brokerIp;
            int numberOfThreads;
            try
            {
                brokerIp = CloudConfigurationManager.GetSetting("BrokerIp");
                
                var threadSet = int.TryParse(CloudConfigurationManager.GetSetting("Threads"), out numberOfThreads);
                if (!threadSet)
                {
                    numberOfThreads = DefaultNumberOfThreads;
                }
            }
            catch (Exception exception)
            {
                brokerIp = string.Empty;
                numberOfThreads = DefaultNumberOfThreads;
                logger.LogException(exception);
            }


            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    logger.LogEvent("Started", "Broker: " + brokerIp);

                    var concurrentConnectonTest = new ConcurrentConnectionTest();
                    var concurrentConnectonTestSetup = new TestSetup(logger, brokerIp, concurrentConnectonTest, numberOfThreads);
                    concurrentConnectonTestSetup.RunThroughputTest(int.MaxValue, new TimeSpan(0, 10, 0), new TimeSpan(0, 1, 0), new TimeSpan(0, 1, 0));

                    logger.LogEvent("Completed", "Completed test using " + numberOfThreads + " threads");
                }
                catch (Exception exception)
                {
                    logger.LogException(exception);
                }

                await Task.Delay(1000);
            }
        }
    }
}
