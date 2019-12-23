using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using MediatR;
using OrderMicroservice.Domain.AggregateModel;
using OrderMicroservice.Models.ResponseModel;
using OrderMicroservice.OptionModel;
using OrderMicroservice.ResponseModel;
using OrderMicroservice.Services.Publisher;

namespace OrderMicroservice.Mediatr.Commands.CreateOrderCommand
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ItemResponse<OrderDTO>>
    {
        private readonly IItemRepository _itemRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly KafkaOrdersService _svc;

        public CreateOrderCommandHandler(IItemRepository itemRepository, IOrderRepository orderRepository, KafkaOrdersService svc)
        {
            _itemRepository = itemRepository;
            _orderRepository = orderRepository;
            _svc = svc;
        }

        public async Task<ItemResponse<OrderDTO>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            
            var orderId = Guid.NewGuid().ToString();
            try
            {
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
                    
                    var kafkaRes = await _svc.CreatePaymentRequest(order, items);
                    if (!kafkaRes.Success)
                    {
                        await _orderRepository.DeleteOrder(order);
                        return new ItemResponse<OrderDTO>
                        {
                            RequestStatus = CustomEnum.RequestStatus.Failed,
                            TransactionTs = DateTime.Now.ToString(),
                            ErrorMessage = "Failed to send message to Payment Service. Your order is not processed."
                        };    
                    }
                    
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