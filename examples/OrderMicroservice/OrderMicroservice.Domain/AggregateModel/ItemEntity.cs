using System;

namespace OrderMicroservice.Domain.AggregateModel
{
    public class ItemEntity
    {
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public ItemType ItemType { get; set; }
        public DateTime DateCreated { get; set; }
        public OrderEntity OrderEntity { get; set; }
    }

    public enum ItemType
    {
        Perishable,
        NonPerishable
    }
}