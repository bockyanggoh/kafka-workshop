using OrderMicroservice.Domain.AggregateModel;

namespace OrderMicroservice.RequestModel
{
    public class CreateItemRequest
    {
        public string Username { get; set; }
        public string ItemName { get; set; }
        public ItemType ItemType { get; set; }
    }
}