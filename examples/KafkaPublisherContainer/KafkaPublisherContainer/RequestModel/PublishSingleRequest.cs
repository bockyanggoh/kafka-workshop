namespace KafkaPublisherContainer.RequestModel
{
    public class PublishSingleRequest
    {
        public string Topic { get; set; }
        public string Message { get; set; }
    }
}