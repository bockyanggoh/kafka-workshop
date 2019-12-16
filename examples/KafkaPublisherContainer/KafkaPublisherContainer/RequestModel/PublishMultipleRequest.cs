using System.Collections.Generic;

namespace KafkaPublisherContainer.RequestModel
{
    public class PublishMultipleRequest
    {
        public string Topic { get; set; }
        public List<string> Message { get; set; }
    }
}