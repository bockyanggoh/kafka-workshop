using System.Collections.Generic;
using MediatR;
using OrderMicroservice.Domain.AggregateModel;
using OrderMicroservice.ResponseModel;

namespace OrderMicroservice.Mediatr.Queries.GetItemsQuery
{
    public class GetItemsQuery : IRequest<ItemResponse<List<ItemEntity>>>
    {
    }
}