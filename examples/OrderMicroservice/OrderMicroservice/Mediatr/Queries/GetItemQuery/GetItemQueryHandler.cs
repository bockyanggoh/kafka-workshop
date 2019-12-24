using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OrderMicroservice.Domain.AggregateModel;
using OrderMicroservice.Models.CustomEnum;
using OrderMicroservice.Models.ResponseModel;

namespace OrderMicroservice.Mediatr.Queries.GetItemQuery
{
    public class GetItemQueryHandler : IRequestHandler<GetItemQuery, ItemResponse<ItemEntity>>
    {
        private readonly IItemRepository _itemRepository;

        public GetItemQueryHandler(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }


        public async Task<ItemResponse<ItemEntity>> Handle(GetItemQuery request, CancellationToken cancellationToken)
        {
            var res = await _itemRepository.FindItemById(request.ItemId);

            return new ItemResponse<ItemEntity>
            {
                RequestStatus = CustomEnum.RequestStatus.Success,
                TransactionTs = DateTime.Now.ToString(),
                ItemData = res
            };
        }
    }
}