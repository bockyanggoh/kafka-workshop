using MediatR;
using OrderMicroservice.Models.ResponseModel;

namespace OrderMicroservice.Mediatr.Commands.SendKafkaMessageCommand
{
    public class SendKafkaMessageCommand<T> : IRequest<KafkaMessageStatus<T>> where T : class
    {
        public string CorrelationId { get; set; }
        public T Message { get; set; }
        public string Topic { get; set; }
        public MessageType MessageType { get; set; }
        public int Partition { get; set; }
        public int Timeout { get; set; }
    }

    public enum MessageType
    {
        Avro,
        Json,
        String
    }
}