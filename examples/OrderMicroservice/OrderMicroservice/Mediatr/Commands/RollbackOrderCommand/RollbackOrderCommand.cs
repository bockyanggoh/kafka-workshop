using System.Collections.Generic;
using System.Security.Policy;
using MediatR;
using OrderMicroservice.Domain.AggregateModel;

namespace OrderMicroservice.Mediatr.Commands.RollbackOrderCommand
{
    public class RollbackOrderCommand: IRequest<bool>
    {
        public OrderEntity Order;
    }
}