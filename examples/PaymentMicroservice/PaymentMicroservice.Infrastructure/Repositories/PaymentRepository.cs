using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public PaymentEntity FindPaymentByOrderId(string orderId)
        {
            var query =  
                from i in _context.Payments
                where i.OrderId == orderId
                select i;
            return query.First();
        }

        public PaymentEntity FindPaymentByPaymentId(string paymentId)
        {
            var query =  
                from i in _context.Payments
                where i.PaymentId == paymentId
                select i;
            return query.First();
        }

        public Task SavePayment(PaymentEntity paymentEntity)
        {
            _context.AddAsync(paymentEntity);
            return _context.SaveChangesAsync();
        }
    }
}