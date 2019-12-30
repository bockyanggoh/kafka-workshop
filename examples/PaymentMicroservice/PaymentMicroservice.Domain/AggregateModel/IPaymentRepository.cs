using System.Threading.Tasks;

namespace PaymentMicroservice.Domain.AggregateModel
{
    public interface IPaymentRepository
    {
        public PaymentEntity FindPaymentByOrderId(string orderId);
        public PaymentEntity FindPaymentByPaymentId(string paymentId);
        public Task SavePayment(PaymentEntity paymentEntity);
    }
}