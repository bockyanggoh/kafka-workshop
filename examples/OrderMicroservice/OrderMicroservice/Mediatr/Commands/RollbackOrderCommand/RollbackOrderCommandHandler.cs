using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OrderMicroservice.Domain.AggregateModel;

namespace OrderMicroservice.Mediatr.Commands.RollbackOrderCommand
{
    public class RollbackOrderCommandHandler: IRequestHandler<RollbackOrderCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;

        public RollbackOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<bool> Handle(RollbackOrderCommand request, CancellationToken cancellationToken)
        {
            await _orderRepository.DeleteOrder(request.Order);
            return true;
        }
    }
}