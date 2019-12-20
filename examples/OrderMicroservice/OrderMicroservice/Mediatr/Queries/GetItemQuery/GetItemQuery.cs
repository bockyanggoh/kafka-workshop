using MediatR;
using OrderMicroservice.Domain.AggregateModel;
using OrderMicroservice.ResponseModel;

namespace OrderMicroservice.Mediatr.Queries.GetItemQuery
{
    public class GetItemQuery : IRequest<ItemResponse<ItemEntity>>
    {
        public string ItemId { get; set; }
    }
}