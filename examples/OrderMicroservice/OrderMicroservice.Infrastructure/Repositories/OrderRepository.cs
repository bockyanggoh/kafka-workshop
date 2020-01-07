using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderMicroservice.Domain.AggregateModel;

namespace OrderMicroservice.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrdersDBContext _context;

        public OrderRepository(OrdersDBContext context)
        {
            _context = context;
        }

        public Task<OrderEntity> FindOrderById(string orderId)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<string>> FindAllOrderIdsByUsername(string username)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<OrderEntity>> FindOrdersByUsername(string username)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateOrderPaymentStatus(string orderId, string paymentStatus)
        {
            var query =
                from o in _context.Orders
                where o.OrderId == orderId
                      select 
        }

        public Task SaveOrder(OrderEntity order)
        {
            _context.Orders.AddAsync(order);
            return _context.SaveChangesAsync();
        }

        public Task SaveOrders(List<OrderEntity> orders)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteOrder(OrderEntity orderEntity)
        {
            _context.Orders.Attach(orderEntity);
            _context.Orders.Remove(orderEntity);
            return _context.SaveChangesAsync();
        }
    }
}