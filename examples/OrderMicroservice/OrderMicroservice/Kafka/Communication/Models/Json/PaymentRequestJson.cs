using System.Collections.Generic;

namespace OrderMicroservice.Kafka.Communication.Models.Json
{
    public class PaymentRequestJson
    {
        public string RequestType { get; set; }
        public string CorrelationId { get; set; }
        public string OrderId { get; set; }
        public string Username { get; set; }
        public string PaymentStatus { get; set; }
        public string RequestedTs { get; set; }
        public IList<ItemCostJson> CostBreakdown { get; set; }
    }

    public class ItemCostJson
    {
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public double CostPrice { get; set; }
    }
}