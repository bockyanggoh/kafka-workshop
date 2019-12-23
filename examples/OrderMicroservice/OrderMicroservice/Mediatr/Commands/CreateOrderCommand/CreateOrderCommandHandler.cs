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
using OrderMicroservice.Services.Subscriber;

namespace OrderMicroservice.Mediatr.Commands.CreateOrderCommand
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ItemResponse<OrderDTO>>
    {
        private readonly IItemRepository _itemRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly SubscribeOrderService _subscribeOrderService;
        private readonly KafkaOrdersService _svc;

        public CreateOrderCommandHandler(IItemRepository itemRepository, IOrderRepository orderRepository, KafkaOrdersService svc, SubscribeOrderService subscribeOrderService)
        {
            _itemRepository = itemRepository;
            _orderRepository = orderRepository;
            _svc = svc;
            _subscribeOrderService = subscribeOrderService;
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
                            ErrorMessage = kafkaRes.ErrorInfo
                        };
                    }
                    var res = await _subscribeOrderService.WaitForMessage(kafkaRes.CorrelationId);

                    if (res.Success)
                    {
                        var itemData = new OrderDTO(order);
                        itemData.PaymentInformation = res.Data.PaymentInformation;
                        return new ItemResponse<OrderDTO>
                        {
                            RequestStatus = CustomEnum.RequestStatus.Success,
                            TransactionTs = DateTime.Now.ToString(),
                            ItemData = new OrderDTO(order),
                        };    
                    }
                    
                    await _orderRepository.DeleteOrder(order);
                    return new ItemResponse<OrderDTO>
                    {
                        RequestStatus = CustomEnum.RequestStatus.Failed,
                        TransactionTs = DateTime.Now.ToString(),
                        ErrorMessage = res.ErrorInfo
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