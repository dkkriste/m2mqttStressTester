namespace MqttStressTester.Utils
{
    using System;
    using System.Collections.Generic;

    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.Azure;

    using MqttStressTester.Contracts;

    public class ApplicationInsightsLogger : ILogger
    {
        private static TelemetryClient applicationInsightsClient;

        public ApplicationInsightsLogger()
        {
            if (applicationInsightsClient == null)
            {
                TelemetryConfiguration.Active.InstrumentationKey =
                    CloudConfigurationManager.GetSetting("Telemetry.AI.InstrumentationKey");
                applicationInsightsClient = new TelemetryClient();
            }
        }

        public void LogException(Exception exception)
        {
            applicationInsightsClient.TrackException(exception);
        }

        public void LogEvent(string eventName, string message)
        {
            var dictionary = new Dictionary<string, string> { { eventName, message } };
            applicationInsightsClient.TrackEvent(eventName, dictionary);
        }

        public void LogMetric(string metricName, double value)
        {
            applicationInsightsClient.TrackMetric(metricName, value);
        }
    }
}