namespace OrderMicroservice.Kafka.Communication.Models.Json
{
    public class PaymentUpdateJson
    {
        public string OrderId { get; set; }
        public PaymentStatus Status { get; set; }
    }

    public enum PaymentStatus
    {
        Paid,
        Unpaid,
        Expired,
    }
}