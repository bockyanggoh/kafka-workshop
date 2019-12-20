using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderMicroservice.Domain.AggregateModel;

namespace OrderMicroservice.Infrastructure.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly OrdersDBContext _context;

        public ItemRepository(OrdersDBContext context)
        {
            _context = context;
        }

        public async Task<ItemEntity> FindItemById(string id)
        {
            var item =
                from i in _context.Items
                where i.ItemId == Guid.Parse(id)
                select i;
            return await item.FirstOrDefaultAsync();
        }

        public async Task<List<ItemEntity>> GetAllItems()
        {
            return await _context.Items.Select(i => i).ToListAsync();
        }

        public Task SaveItem(ItemEntity item)
        {
            _context.Items.AddAsync(item);
            return _context.SaveChangesAsync();
        }

        public Task SaveItems(List<ItemEntity> items)
        {
            _context.Items.AddRangeAsync(items);
            return _context.SaveChangesAsync();
        }
    }
}