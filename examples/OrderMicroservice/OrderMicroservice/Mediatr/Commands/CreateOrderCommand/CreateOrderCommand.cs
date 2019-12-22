using System.Collections.Generic;
using MediatR;
using OrderMicroservice.Domain.AggregateModel;
using OrderMicroservice.Models.ResponseModel;
using OrderMicroservice.ResponseModel;

namespace OrderMicroservice.Mediatr.Commands.CreateOrderCommand
{
    public class CreateOrderCommand : IRequest<ItemResponse<OrderDTO>>
    {
        public string Username { get; set; }
        public List<string> OrderItemIds { get; set; }
    }
}