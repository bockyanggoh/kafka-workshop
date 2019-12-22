using MediatR;
using OrderMicroservice.Domain.AggregateModel;
using OrderMicroservice.Models.ResponseModel;

namespace OrderMicroservice.Mediatr.Commands.CreateItemCommand
{
    public class CreateItemCommand : IRequest<ItemResponse<ItemEntity>>
    {
        public string ItemName { get; set; }
        public ItemType ItemType { get; set; }
        public double CostPrice { get; set; }
    }
}