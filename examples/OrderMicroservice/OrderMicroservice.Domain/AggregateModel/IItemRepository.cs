using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderMicroservice.Domain.AggregateModel
{
    public interface IItemRepository
    {
        public Task<ItemEntity> FindItemById(string id);
        public Task<List<ItemEntity>> GetAllItems();
        public Task SaveItem(ItemEntity item);
        public Task SaveItems(List<ItemEntity> items);
    }
}