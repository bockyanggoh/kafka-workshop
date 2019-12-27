using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OrderMicroservice.Kafka.Services;
using OrderMicroservice.Models.ResponseModel;

namespace OrderMicroservice.Mediatr.Commands.SendKafkaMessageCommand
{
    public class SendKafkaMessageCommandHandler<T>: IRequestHandler<SendKafkaMessageCommand<T>, KafkaMessageStatus<T>> where T : class
    {
        private readonly IKafkaProducer<T> _producer;

        public SendKafkaMessageCommandHandler(IKafkaProducer<T> producer)
        {
            _producer = producer;
        }

        public async Task<KafkaMessageStatus<T>> Handle(SendKafkaMessageCommand<T> request, CancellationToken cancellationToken)
        {
            if (request.MessageType == MessageType.Avro)
            {
                var resAvro = await _producer.SendAvroMessage(
                    request.CorrelationId,
                    request.Message,
                    request.Topic,
                    request.Partition > 0 ? "default" : "5000",
                    request.Timeout
                );

                return resAvro;
            }
            
            var resJson = await _producer.SendJsonMessage(
                request.CorrelationId,
                request.Message,
                request.Topic,
                request.Partition > 0 ? "default" : "5000",
                request.Timeout
            );
            
            return resJson;
        }
    }
}