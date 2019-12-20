using System;

namespace OrderMicroservice.Domain.AggregateModel
{
    public class ItemEntity
    {
        public Guid ItemId { get; set; }
        public string ItemName { get; set; }
        public ItemType ItemType { get; set; }
        public DateTime DateCreated { get; set; }
        public string Username { get; set; }
    }

    public enum ItemType
    {
        Perishable,
        NonPerishable
    }
}