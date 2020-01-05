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
    public class PaymentBackgroundService : KafkaAvroBackgroundConsumer<CreatePaymentRequest>
    {
        private readonly IMediator _mediator;

        public async override Task ProcessingLogic(ConsumeResult<string, CreatePaymentRequest> message)
        {
            var res = await _mediator.Send(new CreatePaymentCommand
            {
                Request = message.Value
            });
        }

        public PaymentBackgroundService(ILogger<PaymentBackgroundService> logger, IOptions<KafkaOptions> options, IMediator mediator) :
            base(logger, options.Value, options.Value.Subscriptions)
        {
            _mediator = mediator;
        }
    }
}