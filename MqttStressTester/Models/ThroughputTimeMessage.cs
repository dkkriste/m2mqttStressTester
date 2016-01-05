namespace MqttStressTester.Models
{
    using System;

    public class ThroughputTimeMessage
    {
        public int MessageNumber { get; set; }

        public DateTimeOffset MessageSendtTime { get; set; }
    }
}