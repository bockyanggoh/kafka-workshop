using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderMicroservice.Domain.AggregateModel
{
    public interface IItemRepository
    {
        public Task<List<ItemEntity>> FindItemsByName(List<string> names);
        public Task<List<ItemEntity>> FindItemsByIds(List<string> itemIds);
        public Task<ItemEntity> FindItemById(string id);
        public Task<List<ItemEntity>> FindAllItems();
        public Task SaveItem(ItemEntity item);
        public Task SaveItems(List<ItemEntity> items);
    }
}