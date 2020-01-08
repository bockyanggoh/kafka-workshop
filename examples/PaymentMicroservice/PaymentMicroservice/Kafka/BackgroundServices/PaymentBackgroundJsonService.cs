using System.Collections.Generic;
using System.Threading.Tasks;
using CAKafka.Domain.Models;
using CAKafka.Library.impl;
using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderMicroservice.Kafka.Communication.Models.Json;
using PaymentMicroservice.Mediatr.Commands.CreatePaymentCommand;

namespace PaymentMicroservice.Kafka.BackgroundServices
{
    public class PaymentBackgroundJsonService : KafkaBackgroundConsumer
    {
        private readonly IMediator _mediator;
        
        public PaymentBackgroundJsonService(ILogger<PaymentBackgroundJsonService> logger, IOptions<KafkaOptions> options, IMediator mediator) :
            base(logger, options.Value, new List<string>{options.Value.Subscriptions[1]})
        {
            _mediator = mediator;
        }
        public override async Task ProcessingLogic(ConsumeResult<string, string> message)
        {
            var converted = JsonConvert.DeserializeObject<PaymentRequestJson>(message.Value);
            var items = new List<Items>();
            foreach (var i in converted.CostBreakdown)
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
                CorrelationId = converted.CorrelationId,
                Username = converted.Username,
                OrderId = converted.OrderId,
                PaymentStatus = converted.PaymentStatus,
                RequestedTs = converted.RequestedTs,
                CostBreakdown = items
            });
        }

        
    }
}