using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OrderMicroservice.Domain.AggregateModel;
using OrderMicroservice.Infrastructure.Repositories;
using OrderMicroservice.ResponseModel;

namespace OrderMicroservice.Mediatr.Commands.CreateItemCommand
{
    public class CreateItemCommandHandler : IRequestHandler<CreateItemCommand, CreateItemResponse<ItemEntity>>
    {
        private readonly ItemRepository _itemRepository;

        public CreateItemCommandHandler(ItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public async Task<CreateItemResponse<ItemEntity>> Handle(CreateItemCommand request, CancellationToken cancellationToken)
        {
            var itemId = Guid.NewGuid();
            try
            {
                var item = new ItemEntity
                {
                    ItemId = itemId.ToString(),
                    DateCreated = DateTime.Now,
                    ItemName = request.ItemName,
                    ItemType = request.ItemType
                };
                await _itemRepository.SaveItem(item);
                
                return new CreateItemResponse<ItemEntity>
                {
                    RequestStatus = "Success",
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