using System.Collections.Generic;
using MediatR;
using OrderMicroservice.Domain.AggregateModel;
using OrderMicroservice.Models.ResponseModel;

namespace OrderMicroservice.Mediatr.Commands.CreateOrderCommand
{
    public class CreateOrderCommand : IRequest<ItemResponse<OrderEntity>>
    {
        public string Username { get; set; }
        public List<string> OrderItemIds { get; set; }
    }
}