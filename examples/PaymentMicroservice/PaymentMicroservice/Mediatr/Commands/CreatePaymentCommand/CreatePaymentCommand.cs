using Kafka.Communication.Models;
using MediatR;
using PaymentMicroservice.Domain.AggregateModel;

namespace PaymentMicroservice.Mediatr.Commands.CreatePaymentCommand
{
    public class CreatePaymentCommand: IRequest<PaymentEntity>
    {
        public CreatePaymentRequest Request { get; set; }
    }
}