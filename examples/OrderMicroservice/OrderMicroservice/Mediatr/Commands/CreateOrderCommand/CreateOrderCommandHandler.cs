using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OrderMicroservice.Domain.AggregateModel;
using OrderMicroservice.ResponseModel;

namespace OrderMicroservice.Mediatr.Commands.CreateOrderCommand
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ItemResponse<OrderEntity>>
    {
        public Task<ItemResponse<OrderEntity>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}