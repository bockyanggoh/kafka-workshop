using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaymentMicroservice.Domain.AggregateModel;
using PaymentMicroservice.Infrastructure.Repositories;

namespace PaymentMicroservice.Mediatr.Queries.FindPaymentQuery
{
    public class FindPaymentQueryHandler: IRequestHandler<FindPaymentQuery, PaymentEntity>
    {
        private readonly PaymentRepository _paymentRepository;

        public FindPaymentQueryHandler(PaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public Task<PaymentEntity> Handle(FindPaymentQuery request, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                try
                {
                    var res = string.IsNullOrEmpty(request.OrderId)
                        ? _paymentRepository.FindPaymentByPaymentId(request.PaymentId)
                        : _paymentRepository.FindPaymentByOrderId(request.OrderId);
                    return res;
                }
                catch (Exception)
                {
                    return null;
                }
            });

        }
    }
}