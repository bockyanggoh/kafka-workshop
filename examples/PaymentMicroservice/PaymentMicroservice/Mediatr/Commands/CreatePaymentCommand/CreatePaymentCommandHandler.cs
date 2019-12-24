using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaymentMicroservice.Domain.AggregateModel;

namespace PaymentMicroservice.Mediatr.Commands.CreatePaymentCommand
{
    public class CreatePaymentCommandHandler: IRequestHandler<CreatePaymentCommand, PaymentEntity>
    {

        private readonly IPaymentRepository _paymentRepository;

        public CreatePaymentCommandHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<PaymentEntity> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {

            var totalCost = request.Request.CostBreakdown.Select(i => i.CostPrice).Sum();
            var record = new PaymentEntity
            {
                PaymentId = Guid.NewGuid().ToString(),
                OrderId = request.Request.OrderId,
                PaymentStatus = (PaymentStatus)Enum.Parse(typeof(PaymentStatus), request.Request.PaymentStatus),
                CreatedTs = DateTime.Now,
                AmountPayable = totalCost,
                Username = request.Request.Username
            };

            await _paymentRepository.SavePayment(record);
            return record;
        }
    }
}