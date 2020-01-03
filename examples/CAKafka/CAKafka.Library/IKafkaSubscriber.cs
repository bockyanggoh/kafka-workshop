using System.Threading.Tasks;
using CAKafka.Domain.Models;

namespace CAKafka.Library
{
    public interface IKafkaSubscriber<T> where T : class
    {
        public Task<KafkaMessageStatus<T>> ReadJsonMessage(string corrId, string topic, int timeout=5000);
        public Task<KafkaMessageStatus<T>> ReadAvroMessage(string corrId, string topic, int timeout=5000);
    }
}