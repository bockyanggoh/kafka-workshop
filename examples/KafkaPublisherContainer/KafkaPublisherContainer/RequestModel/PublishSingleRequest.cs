using System.Collections.Generic;

namespace KafkaPublisherContainer.RequestModel
{
    public class PublishSingleRequest
    {
        public string Topic { get; set; }
        public string Brokers { get; set; }
        public string Message { get; set; }
    }
}