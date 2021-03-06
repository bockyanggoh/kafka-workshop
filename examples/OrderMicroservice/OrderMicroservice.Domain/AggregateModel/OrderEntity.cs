using System;
using System.Collections.Generic;

namespace OrderMicroservice.Domain.AggregateModel
{
    public class OrderEntity
    {
        public string OrderId { get; set; }
        public string Username { get; set; }
        public DateTime CreatedTs { get; set; }
        public List<OrderItemEntity> OrderItems { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
    }

    public enum PaymentStatus
    {
        Paid,
        Pending,
        Rejected
    }

    public class OrderItemEntity
    {
        public string OrderItemId { get; set; }
        public string OrderId { get; set; }
        public OrderEntity OrderEntity { get; set; }
        public string ItemId { get; set; }
    }
}