using MediatR;
using PaymentMicroservice.Domain.AggregateModel;

namespace PaymentMicroservice.Mediatr.Queries.FindPaymentQuery
{
    public class FindPaymentQuery: IRequest<PaymentEntity>
    {
        public string OrderId { get; set; }
        public string PaymentId { get; set; }
    }
}