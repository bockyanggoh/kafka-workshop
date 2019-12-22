using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.OpenApi.Extensions;
using OrderMicroservice.Domain.AggregateModel;
using OrderMicroservice.Models.ResponseModel;

namespace OrderMicroservice.Mediatr.Commands.CreateItemsCommand
{
    public class CreateItemsCommandHandler : IRequestHandler<CreateItemsCommand, ItemResponse<List<ItemEntity>>>
    {
        private readonly IItemRepository _itemRepository;

        public CreateItemsCommandHandler(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public async Task<ItemResponse<List<ItemEntity>>> Handle(CreateItemsCommand request, CancellationToken cancellationToken)
        {
            List<ItemEntity> entities = new List<ItemEntity>();
            DateTime currentTime = DateTime.Now;
            foreach (var item in request.Request.Items)
            {
                entities.Add(new ItemEntity
                {
                    ItemId = Guid.NewGuid().ToString(),
                    ItemName = item.ItemName,
                    ItemType = item.ItemType,
                    CostPrice = item.CostPrice,
                    DateCreated = currentTime
                });
            }

            try
            {
                await _itemRepository.SaveItems(entities);
                return new ItemResponse<List<ItemEntity>>
                {
                    RequestStatus = CustomEnum.RequestStatus.Success,
                    TransactionTs = DateTime.Now.ToString(),
                    ItemData = entities
                };
            }
            catch (Exception)
            {
                var items = request.Request.Items.Select(i => i.ItemName).ToList();
                var res = await _itemRepository.FindItemsByName(items);
                return new ItemResponse<List<ItemEntity>>
                {
                    RequestStatus = CustomEnum.RequestStatus.Success,
                    TransactionTs = DateTime.Now.ToString(),
                    ItemData = res
                };
            }
        }
    }
}