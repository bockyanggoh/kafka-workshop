using System;
using System.Collections.Generic;
using Kafka.Communication.Models;
using Microsoft.OpenApi.Extensions;
using OrderMicroservice.Domain.AggregateModel;

namespace OrderMicroservice.ResponseModel
{
    public class OrderDTO
    {
        public string OrderId { get; set; }
        public string Username { get; set; }
        public DateTime CreatedTs { get; set; }
        public List<OrderItemDTO> OrderItems { get; set; }
        public PaymentInformation PaymentInformation { get; set; }
        public string PaymentStatus { get; set; }

        public OrderDTO(OrderEntity orderEntity)
        {
            OrderId = orderEntity.OrderId;
            Username = orderEntity.Username;
            CreatedTs = orderEntity.CreatedTs;
            var items = new List<OrderItemDTO>();
            foreach (var i in orderEntity.OrderItems)
            {
                items.Add(new OrderItemDTO
                {
                    OrderItemId = i.OrderItemId
                });
            }
            OrderItems = items;
            PaymentStatus = orderEntity.PaymentStatus.GetDisplayName();
        }
    }

    public class OrderItemDTO
    {
        public string OrderItemId { get; set; }
        public string OrderItemName { get; set; }
        public double CostPrice { get; set; }
        public string ItemType { get; set; }
        public DateTime CreatedTs { get; set; }
    }
}