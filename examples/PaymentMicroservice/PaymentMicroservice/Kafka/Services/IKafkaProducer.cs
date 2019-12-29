using System.Threading.Tasks;
using OrderMicroservice.Models.ResponseModel;

namespace OrderMicroservice.Kafka.Services
{
    public interface IKafkaProducer<T> where T : class
    {
        public Task<KafkaMessageStatus<T>> SendJsonMessage(string corrId, T message, string topic, string partition="default", int timout=5000);
        public Task<KafkaMessageStatus<T>> SendAvroMessage(string corrId, T message, string topic, string partition="default", int timeout = 5000);
    }
}