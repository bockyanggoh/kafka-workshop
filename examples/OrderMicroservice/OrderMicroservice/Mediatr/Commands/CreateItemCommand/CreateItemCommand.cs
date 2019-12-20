using MediatR;
using OrderMicroservice.Domain.AggregateModel;
using OrderMicroservice.ResponseModel;

namespace OrderMicroservice.Mediatr.Commands.CreateItemCommand
{
    public class CreateItemCommand : IRequest<CreateItemResponse<ItemEntity>>
    {
        public string ItemName { get; set; }
        public ItemType ItemType { get; set; }
        public string Username { get; set; }
    }
}