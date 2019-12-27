using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OrderMicroservice.Kafka.Services;
using OrderMicroservice.Mediatr.Commands.SendKafkaMessageCommand;
using OrderMicroservice.Models.ResponseModel;

namespace OrderMicroservice.Mediatr.Queries.ReceiveKafkaMessageQuery
{
    public class ReceiveKafkaMessageQueryHandler<T>: IRequestHandler<ReceiveKafkaMessageQuery<T>, KafkaMessageStatus<T>> where T : class
    {
        private readonly IKafkaSubscriber<T> _subscriber;

        public ReceiveKafkaMessageQueryHandler(IKafkaSubscriber<T> subscriber)
        {
            _subscriber = subscriber;
        }

        public async Task<KafkaMessageStatus<T>> Handle(ReceiveKafkaMessageQuery<T> request, CancellationToken cancellationToken)
        {
            if (request.MessageType == MessageType.Avro)
            {
                var resAvro = await _subscriber.ReadAvroMessage(
                    request.CorrelationId, 
                    request.Topic,
                    request.Timeout > 0 ? request.Timeout: 5000);
                return resAvro;
            }

            var resJson = await _subscriber.ReadJsonMessage(request.CorrelationId, request.Topic);
            return resJson;
        }
    }
}