using System.Collections.Generic;
using System.Threading.Tasks;
using CAKafka.Domain.Models;
using CAKafka.Library.impl;
using Confluent.Kafka;
using Kafka.Communication.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OrderMicroservice.Kafka.BackgroundService
{
    public class PaymentBackgroundAvroService : KafkaAvroBackgroundConsumer<CreatePaymentResponse>
    {
        private readonly ILogger<PaymentBackgroundAvroService> _logger;
        private readonly IMediator _mediator;

        public PaymentBackgroundAvroService(IOptions<KafkaOptions> options, IMediator mediator,
            ILogger<PaymentBackgroundAvroService> logger) :
            base(logger, options.Value, new List<string> {options.Value.Subscriptions[0]})
        {
            _logger = logger;
            _mediator = mediator;
        }

        public override Task ProcessingLogic(ConsumeResult<string, CreatePaymentResponse> message)
        {
            _logger.LogInformation($"Message Key: {message.Key}");
            return Task.CompletedTask;
        }
    }
}