using OrderMicroservice.Domain.AggregateModel;

namespace OrderMicroservice.RequestModel
{
    public class CreateItemRequest
    {
        public string ItemName { get; set; }
        public ItemType ItemType { get; set; }
        public double CostPrice { get; set; }
    }
}