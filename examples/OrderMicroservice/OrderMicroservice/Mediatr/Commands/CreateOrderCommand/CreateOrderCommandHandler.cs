using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CAKafka.Domain.Models;
using CAKafka.Library;
using Kafka.Communication.Models;
using MediatR;
using OrderMicroservice.Domain.AggregateModel;
using OrderMicroservice.Kafka.Communication.Models.Json;
using OrderMicroservice.Models.CustomEnum;
using OrderMicroservice.Models.ResponseModel;
using OrderMicroservice.ResponseModel;
using PaymentStatus = OrderMicroservice.Domain.AggregateModel.PaymentStatus;

namespace OrderMicroservice.Mediatr.Commands.CreateOrderCommand
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ItemResponse<OrderDTO>>
    {
        private readonly IItemRepository _itemRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IKafkaMessageService<CreatePaymentRequest, CreatePaymentResponse> _kafkaMessageService;
        private readonly IKafkaProducer<PaymentRequestJson> _kafkaJsonSvc;
        private readonly IMediator _mediator;

        public CreateOrderCommandHandler(
            IItemRepository itemRepository, 
            IOrderRepository orderRepository,
            IKafkaMessageService<CreatePaymentRequest, CreatePaymentResponse> kafkaMessageService,
            IKafkaProducer<PaymentRequestJson> kafkaJsonSvc,
            IMediator mediator)
        {   
            _mediator = mediator;
            _kafkaMessageService = kafkaMessageService;
            _kafkaJsonSvc = kafkaJsonSvc;
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

                    if (request.MessageType == KafkaMethods.MessageType.Avro)
                    {
                        
                        var costBreakdown = new List<ItemCost>();
                        foreach (var i in items)
                        {
                            costBreakdown.Add(new ItemCost
                            {
                                ItemId = i.ItemId,
                                ItemName = i.ItemName,
                                CostPrice = i.CostPrice
                            });
                        }
                        var paymentRequest = new CreatePaymentRequest
                        {
                            Username = order.Username,
                            PaymentStatus = PaymentStatus.Pending.ToString(),
                            CorrelationId = order.OrderId,
                            OrderId = order.OrderId,
                            RequestedTs = order.CreatedTs.ToString(),
                            RequestType = "Create",
                            CostBreakdown = costBreakdown
                        };
                        await _kafkaMessageService.SendMessage(
                            new KafkaMessageDetails<CreatePaymentRequest>
                            {
                                CorrelationId = paymentRequest.CorrelationId,
                                Message = paymentRequest,
                                MessageType = MessageType.Avro,
                                Partition = 0,
                                Timeout = 8000,
                                Topic = "PaymentRequestAvro",
                                ResponseTopic = "PaymentResponseAvro"
                            });
                    }
                    else if (request.MessageType == KafkaMethods.MessageType.Json ||
                             request.MessageType == KafkaMethods.MessageType.String)
                    {
                        var costBreakdown = new List<ItemCostJson>();
                        foreach (var i in items)
                        {
                            costBreakdown.Add(new ItemCostJson
                            {
                                ItemId = i.ItemId,
                                ItemName = i.ItemName,
                                CostPrice = i.CostPrice
                            });
                        }

                        var paymentRequest = new PaymentRequestJson
                        {
                            Username = order.Username,
                            PaymentStatus = PaymentStatus.Pending.ToString(),
                            CorrelationId = order.OrderId,
                            OrderId = order.OrderId,
                            RequestedTs = order.CreatedTs.ToString(),
                            RequestType = "Create",
                            CostBreakdown = costBreakdown
                        };

                        await _kafkaJsonSvc.SendJsonMessage(paymentRequest.OrderId, paymentRequest,
                            "PaymentRequestJson");

                    }

                    else
                    {
                        throw new NotImplementedException("Byte support to be implemented.");
                    }
                    
                    var itemData = new OrderDTO(order);
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