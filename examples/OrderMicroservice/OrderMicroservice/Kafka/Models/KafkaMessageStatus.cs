namespace OrderMicroservice.Kafka.Models
{
    public class KafkaMessageStatus<T> where T : class
    {
        public string CorrelationId { get; set; }
        public bool Success { get; set; }
        public string ErrorInfo { get; set; }
        public T Data { get; set; }
        public int Partition { get; set; }
        public int Offset { get; set; }
    }
}