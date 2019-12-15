using System.Collections.Generic;

namespace KafkaBasicPublisher.RequestModel
{
    public class PublishMultipleRequest
    {
        public List<string> Message { get; set; }
    }
}