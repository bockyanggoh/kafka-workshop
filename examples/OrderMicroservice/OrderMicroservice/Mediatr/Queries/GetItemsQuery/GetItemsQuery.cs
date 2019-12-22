using System.Collections.Generic;
using MediatR;
using OrderMicroservice.Domain.AggregateModel;
using OrderMicroservice.Models.ResponseModel;

namespace OrderMicroservice.Mediatr.Queries.GetItemsQuery
{
    public class GetItemsQuery : IRequest<ItemResponse<List<ItemEntity>>>
    {
    }
}