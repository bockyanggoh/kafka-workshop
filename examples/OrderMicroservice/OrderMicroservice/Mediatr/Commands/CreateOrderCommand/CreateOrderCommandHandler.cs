using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OrderMicroservice.Domain.AggregateModel;
using OrderMicroservice.Models.ResponseModel;
using OrderMicroservice.ResponseModel;

namespace OrderMicroservice.Mediatr.Commands.CreateOrderCommand
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ItemResponse<OrderDTO>>
    {
        private readonly IItemRepository _itemRepository;
        private readonly IOrderRepository _orderRepository;

        public CreateOrderCommandHandler(IItemRepository itemRepository, IOrderRepository orderRepository)
        {
            _itemRepository = itemRepository;
            _orderRepository = orderRepository;
        }

        public async Task<ItemResponse<OrderDTO>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var orderId = Guid.NewGuid().ToString();
                var items = await _itemRepository.FindItemsByIds(request.OrderItemIds);

                if (items.Count == request.OrderItemIds.Count)
                {
                    var order = new OrderEntity
                    {
                        Username = request.Username,
                        OrderId = orderId,
                        CreatedTs = DateTime.Now,
                        PaymentStatus = PaymentStatus.Pending
                    };
                    var orderItems = new List<OrderItemEntity>();
                    foreach (var i in items)
                    {
                        orderItems.Add(new OrderItemEntity
                        {
                            ItemId = i.ItemId,
                            OrderItemId = Guid.NewGuid().ToString(),
                            OrderId = orderId,
                            OrderEntity = order
                        });
                    }

                    order.OrderItems = orderItems;

                    await _orderRepository.SaveOrder(order);
                    return new ItemResponse<OrderDTO>
                    {
                        RequestStatus = CustomEnum.RequestStatus.Success,
                        TransactionTs = DateTime.Now.ToString(),
                        ItemData = new OrderDTO(order)
                    };
                }
                return new ItemResponse<OrderDTO>
                {
                    RequestStatus = CustomEnum.RequestStatus.Failed,
                    TransactionTs = DateTime.Now.ToString(),
                    ErrorMessage = "Some ItemIds are invalid. Please ensure requested items exist."
                };
            }
            catch (Exception e)
            {
                return new ItemResponse<OrderDTO>
                {
                    RequestStatus = CustomEnum.RequestStatus.Failed,
                    TransactionTs = DateTime.Now.ToString(),
                    ErrorMessage = e.Message
                };
            }
        }
    }
}