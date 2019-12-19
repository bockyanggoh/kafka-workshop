using System.Collections.Generic;

namespace KafkaPublisherAvro.RequestModel
{
    public class PublishMultipleRequest
    {
        public string Topic { get; set; }
        public string Brokers { get; set; }
        public List<JustAMessage> Messages { get; set; }
    }
}