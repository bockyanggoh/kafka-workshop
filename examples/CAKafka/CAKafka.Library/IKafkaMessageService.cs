using System.Threading.Tasks;
using CAKafka.Domain.Models;

namespace CAKafka.Library
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