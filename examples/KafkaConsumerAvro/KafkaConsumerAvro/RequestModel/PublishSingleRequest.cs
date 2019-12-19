namespace KafkaPublisherAvro.RequestModel
{
    public class PublishSingleRequest
    {
        public string Topic { get; set; }
        public string Brokers { get; set; }
        public JustAMessage Message { get; set; }
    }
}