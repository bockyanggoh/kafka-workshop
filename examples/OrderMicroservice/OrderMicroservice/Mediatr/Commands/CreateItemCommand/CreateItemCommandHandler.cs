using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OrderMicroservice.Domain.AggregateModel;
using OrderMicroservice.Models.ResponseModel;

namespace OrderMicroservice.Mediatr.Commands.CreateItemCommand
{
    public class CreateItemCommandHandler : IRequestHandler<CreateItemCommand, ItemResponse<ItemEntity>>
    {
        private readonly IItemRepository _itemRepository;

        public CreateItemCommandHandler(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public async Task<ItemResponse<ItemEntity>> Handle(CreateItemCommand request, CancellationToken cancellationToken)
        {
            var itemId = Guid.NewGuid();
            try
            {
                var item = new ItemEntity
                {
                    ItemId = itemId.ToString(),
                    DateCreated = DateTime.Now,
                    ItemName = request.ItemName,
                    ItemType = request.ItemType,
                    CostPrice = request.CostPrice
                };
                await _itemRepository.SaveItem(item);
                
                return new ItemResponse<ItemEntity>
                {
                    RequestStatus = CustomEnum.RequestStatus.Success,
                    TransactionTs = item.DateCreated.ToString(),
                    ItemData = item
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}