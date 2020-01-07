using System.Collections.Generic;
using System.Threading.Tasks;
using CAKafka.Domain.Models;
using CAKafka.Library.impl;
using Confluent.Kafka;
using Kafka.Communication.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentMicroservice.Mediatr.Commands.CreatePaymentCommand;

namespace PaymentMicroservice.Kafka.BackgroundServices
{
    public class PaymentBackgroundAvroService : KafkaAvroBackgroundConsumer<CreatePaymentRequest>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PaymentBackgroundAvroService> _logger;

        public async override Task ProcessingLogic(ConsumeResult<string, CreatePaymentRequest> message)
        {
            var items = new List<Items>();
            foreach (var i in message.Value.CostBreakdown)
            {
                items.Add(new Items
                {
                    CostPrice = i.CostPrice,
                    ItemId =  i.ItemId,
                    ItemName = i.ItemName
                });
            }
            var res = await _mediator.Send(new CreatePaymentCommand
            {
                CorrelationId = message.Value.CorrelationId,
                Username = message.Value.Username,
                OrderId = message.Value.OrderId,
                PaymentStatus = message.Value.PaymentStatus,
                RequestedTs = message.Value.RequestedTs,
                CostBreakdown = items
            });
            
            
        }

        public PaymentBackgroundAvroService(ILogger<PaymentBackgroundAvroService> logger, IOptions<KafkaOptions> options, IMediator mediator) :
            base(logger, options.Value, new List<string>{options.Value.Subscriptions[0]})
        {
            _logger = logger;
            _mediator = mediator;
        }
    }
}