using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OrderMicroservice.Domain.AggregateModel;
using OrderMicroservice.Infrastructure.Repositories;
using OrderMicroservice.Models.CustomEnum;
using OrderMicroservice.Models.ResponseModel;

namespace OrderMicroservice.Mediatr.Queries.GetItemsQuery
{
    public class GetItemsQueryHandler : IRequestHandler<GetItemsQuery, ItemResponse<List<ItemEntity>>>
    {
        private readonly IItemRepository _itemRepository;

        public GetItemsQueryHandler(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public async Task<ItemResponse<List<ItemEntity>>> Handle(GetItemsQuery request, CancellationToken cancellationToken)
        {
            var res = await _itemRepository.FindAllItems();
            return new ItemResponse<List<ItemEntity>>
            {
                RequestStatus = CustomEnum.RequestStatus.Success,
                TransactionTs = DateTime.Now.ToString(),
                ItemData = res
            };
        }
    }
}