using System.Collections.Generic;
using MediatR;
using OrderMicroservice.Domain.AggregateModel;
using OrderMicroservice.Models.ResponseModel;
using OrderMicroservice.RequestModel;

namespace OrderMicroservice.Mediatr.Commands.CreateItemsCommand
{
    public class CreateItemsCommand : IRequest<ItemResponse<List<ItemEntity>>>
    {
        public CreateItemsRequest Request { get; set; }
    }
}