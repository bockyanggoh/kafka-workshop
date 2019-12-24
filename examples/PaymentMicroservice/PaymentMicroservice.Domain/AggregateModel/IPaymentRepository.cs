using System.Threading.Tasks;

namespace PaymentMicroservice.Domain.AggregateModel
{
    public interface IPaymentRepository
    {
        public Task<PaymentEntity> FindPaymentByOrderId(string orderId);
        public Task<PaymentEntity> FindPaymentByPaymentId(string paymentId);
        public Task SavePayment(PaymentEntity paymentEntity);
    }
}