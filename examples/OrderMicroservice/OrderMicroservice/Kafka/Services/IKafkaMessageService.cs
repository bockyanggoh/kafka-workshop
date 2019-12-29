using System.Threading.Tasks;
using OrderMicroservice.Kafka.Models;

namespace OrderMicroservice.Kafka.Services
{
    public interface IKafkaMessageService<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        public Task<KafkaMessageStatus<TResponse>> SendAndReceiveMessage(KafkaMessageDetails<TRequest> details);
        public Task SendMessage(KafkaMessageDetails<TRequest> details);
        public Task<KafkaMessageStatus<TResponse>> ReceiveMessage(string topic, MessageType messageType, string keyOfNotice = "default");
    }
}