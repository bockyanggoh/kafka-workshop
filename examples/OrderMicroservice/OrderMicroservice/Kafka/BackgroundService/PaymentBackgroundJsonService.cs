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

namespace OrderMicroservice.Kafka.BackgroundService
{
    public class PaymentBackgroundJsonService : KafkaBackgroundConsumer
    {
        private ILogger<PaymentBackgroundJsonService> _logger;
        private IMediator _mediator;
        public PaymentBackgroundJsonService(ILogger<PaymentBackgroundJsonService> logger, IOptions<KafkaOptions> options, IMediator mediator) : 
            base(logger, options.Value, new List<string>{options.Value.Subscriptions[1]})
        {
            _logger = logger;
            _mediator = mediator;
        }

        public override Task ProcessingLogic(ConsumeResult<string, string> message)
        {
            return Task.Run(() =>
            {
                try
                {
                    var mappedMessage = JsonConvert.DeserializeObject<PaymentUpdateJson>(message.Value);
                    
                }
                catch (JsonException e)
                {
                    
                }
            });
        }
    }
}