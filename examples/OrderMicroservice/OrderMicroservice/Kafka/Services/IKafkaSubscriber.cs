using System.Threading.Tasks;
using OrderMicroservice.Kafka.Models;
using OrderMicroservice.Models.ResponseModel;

namespace OrderMicroservice.Kafka.Services
{
    public interface IKafkaSubscriber<T> where T : class
    {
        public Task<KafkaMessageStatus<T>> ReadJsonMessage(string corrId, string topic, int timeout=5000);
        public Task<KafkaMessageStatus<T>> ReadAvroMessage(string corrId, string topic, int timeout=5000);
    }
}