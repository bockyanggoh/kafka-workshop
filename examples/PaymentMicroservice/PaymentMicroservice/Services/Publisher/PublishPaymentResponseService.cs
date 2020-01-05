using System;
using System.Threading.Tasks;
using CAKafka.Domain.Models;
using Kafka.Communication.Models;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Extensions;
using PaymentMicroservice.Domain.AggregateModel;

namespace PaymentMicroservice.Services.Publisher
{
    public class PublishPaymentResponseService: BaseKafkaService
    {
        public PublishPaymentResponseService(IOptions<KafkaOptions> option) : base(option)
        {
        }

        public async Task SendPaymentResponse(PaymentEntity paymentEntity, string corrId)
        {
            var msg = new CreatePaymentResponse
            {
                CorrelationId = corrId,
                OrderId = paymentEntity.OrderId,
                PaymentInformation = new PaymentInformation
                {
                    PaymentAmount = paymentEntity.AmountPayable,
                    PaymentId = paymentEntity.PaymentId,
                    PaymentStatus = paymentEntity.PaymentStatus.GetDisplayName()
                }
            };

            await SendMessage(msg, corrId);
        }
    }
}