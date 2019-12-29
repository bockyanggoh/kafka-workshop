namespace OrderMicroservice.Kafka.Models
{
    public class KafkaMessageDetails<T> where T : class
    {
        public string CorrelationId { get; set; }
        public T Message { get; set; }
        public string Topic { get; set; }
        public string ResponseTopic { get; set; }
        public MessageType MessageType { get; set; }
        public int Partition { get; set; }
        public int Timeout { get; set; }
    }

    public enum MessageType
    {
        Avro,
        Json,
        String
    }
}