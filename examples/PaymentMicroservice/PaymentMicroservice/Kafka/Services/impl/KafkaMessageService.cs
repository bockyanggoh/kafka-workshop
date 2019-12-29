using System.Threading.Tasks;
using PaymentMicroservice.Kafka.Models;

namespace PaymentMicroservice.Kafka.Services.impl
{
    public class KafkaMessageService<TRequest, TResponse> : IKafkaMessageService<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        private readonly IKafkaProducer<TRequest> _producer;
        private readonly IKafkaSubscriber<TResponse> _subscriber;

        public KafkaMessageService(IKafkaProducer<TRequest> producer, IKafkaSubscriber<TResponse> subscriber)
        {
            _producer = producer;
            _subscriber = subscriber;
        }

        public async Task<KafkaMessageStatus<TResponse>> SendAndReceiveMessage(KafkaMessageDetails<TRequest> details)
        {
            await SendMessage(details);
            var messageRes = await ReceiveMessage(
                details.Topic,
                details.MessageType,
                string.IsNullOrEmpty(details.CorrelationId) ? "default" : details.CorrelationId);
            return messageRes;
        }

        public async Task SendMessage(KafkaMessageDetails<TRequest> details)
        {
            if (details.MessageType == MessageType.Avro)
            {
                await _producer.SendAvroMessage(
                    details.CorrelationId,
                    details.Message,
                    details.Topic
                );
            }
            
            await _producer.SendJsonMessage(
                details.CorrelationId,
                details.Message,
                details.Topic
            );
        }

        public async Task<KafkaMessageStatus<TResponse>> ReceiveMessage(string topic, MessageType messageType, string keyOfNotice = "default")
        {
            if (messageType == MessageType.Avro)
            {
                var resAvro = await _subscriber.ReadAvroMessage(
                    keyOfNotice, 
                    topic,
                    5000);
                return resAvro;
            }

            var resJson = await _subscriber.ReadJsonMessage(keyOfNotice, topic);
            return resJson;
        }
    }
}