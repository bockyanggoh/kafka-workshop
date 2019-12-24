using System;
using System.Threading.Tasks;
using PaymentMicroservice.Domain.AggregateModel;

namespace PaymentMicroservice.Infrastructure.Repositories
{
    public class PaymentRepository: IPaymentRepository
    {
        private readonly PaymentDBContext _context;

        public PaymentRepository(PaymentDBContext context)
        {
            _context = context;
        }

        public Task<PaymentEntity> FindPaymentByOrderId(string orderId)
        {
            throw new NotImplementedException();
        }

        public Task<PaymentEntity> FindPaymentByPaymentId(string paymentId)
        {
            throw new NotImplementedException();
        }

        public Task SavePayment(PaymentEntity paymentEntity)
        {
            _context.AddAsync(paymentEntity);
            return _context.SaveChangesAsync();
        }
    }
}