using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kafka.Communication.Models;
using MediatR;
using OrderMicroservice.Domain.AggregateModel;
using OrderMicroservice.Models.CustomEnum;
using OrderMicroservice.Models.ResponseModel;
using OrderMicroservice.ResponseModel;
using OrderMicroservice.Services;

namespace OrderMicroservice.Mediatr.Commands.CreateOrderCommand
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ItemResponse<OrderDTO>>
    {
        private readonly IItemRepository _itemRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IKafkaMessageService<CreatePaymentRequest, CreatePaymentResponse> _kafkaMessageService;
        private readonly IMediator _mediator;

        public CreateOrderCommandHandler(
            IItemRepository itemRepository, 
            IOrderRepository orderRepository,
            IKafkaMessageService<CreatePaymentRequest, CreatePaymentResponse> kafkaMessageService,
            IMediator mediator)
        {   
            _mediator = mediator;
            _kafkaMessageService = kafkaMessageService;
            _itemRepository = itemRepository;
            _orderRepository = orderRepository;
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
                    var costBreakdown = new List<ItemCost>();
                    
                    foreach (var i in items)
                    {
                        orderItems.Add(new OrderItemEntity
                        {
                            ItemId = i.ItemId,
                            OrderItemId = Guid.NewGuid().ToString(),
                            OrderId = orderId,
                            OrderEntity = order
                        });
                        costBreakdown.Add(new ItemCost
                        {
                            ItemId = i.ItemId,
                            ItemName = i.ItemName,
                            CostPrice = i.CostPrice
                        });
                    }

                    order.OrderItems = orderItems;

                    await _orderRepository.SaveOrder(order);
                    
                    var paymentRequest = new CreatePaymentRequest
                    {
                        Username = order.Username,
                        PaymentStatus = PaymentStatus.Pending.ToString(),
                        OrderId = order.OrderId,
                        RequestedTs = order.CreatedTs.ToString(),
                        RequestType = "Create",
                        CostBreakdown = costBreakdown
                    };
                    var kafkaRes = await _kafkaMessageService.SendAndReceiveMessage(
                        new KafkaMessageDetails<CreatePaymentRequest>
                    {
                        CorrelationId = paymentRequest.CorrelationId,
                        Message = paymentRequest,
                        MessageType = MessageType.Avro,
                        Partition = 0,
                        Timeout = 8000,
                        Topic = "CreatePaymentRequestAvro"
                    });
                    
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

                    

                    var itemData = new OrderDTO(order);
                    itemData.PaymentInformation = kafkaRes.Data.PaymentInformation;
                    return new ItemResponse<OrderDTO>
                    {
                        RequestStatus = CustomEnum.RequestStatus.Success,
                        TransactionTs = DateTime.Now.ToString(),
                        ItemData = itemData
                    };    
                }
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
            
            return new ItemResponse<OrderDTO>
            {
                RequestStatus = CustomEnum.RequestStatus.Failed,
                TransactionTs = DateTime.Now.ToString(),
                ErrorMessage = "Unhandled error"
            };
        }
    }
}