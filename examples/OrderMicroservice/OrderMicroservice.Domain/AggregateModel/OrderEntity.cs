using System;
using System.Collections.Generic;

namespace OrderMicroservice.Domain.AggregateModel
{
    public class OrderEntity
    {
        public Guid OrderId { get; set; }
        public string Username { get; set; }
        public DateTime CreatedTs { get; set; }
        public List<ItemEntity> Items { get; set; }
    }
}