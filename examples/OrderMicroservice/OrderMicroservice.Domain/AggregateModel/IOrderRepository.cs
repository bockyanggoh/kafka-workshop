using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderMicroservice.Domain.AggregateModel
{
    public interface IOrderRepository
    {
        public Task<OrderEntity> FindOrderById(string orderId);

        public Task<List<string>> FindAllOrderIdsByUsername(string username);

        public Task<List<OrderEntity>> FindOrdersByUsername(string username);

        public Task SaveOrder(OrderEntity order);
        public Task SaveOrders(List<OrderEntity> orders);
    }
}