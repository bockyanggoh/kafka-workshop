namespace OrderMicroservice.ResponseModel
{
    public class KafkaPublishStatus
    {
        public string Status { get; set; }
        public bool Success { get; set; }
        public string CorrelationId { get; set; }
    }
}