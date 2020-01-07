using MediatR;
using OrderMicroservice.Models.ResponseModel;
using OrderMicroservice.ResponseModel;

namespace OrderMicroservice.Mediatr.Commands.UpdateOrderStatusCommand
{
    public class UpdateOrderStatusCommand : IRequest<ItemResponse<OrderDTO>>
    {
        public string OrderId { get; set; }
        public string PaymentStatus { get; set; }
    }
}