using MediatR;
using OrderMicroservice.Mediatr.Commands.SendKafkaMessageCommand;
using OrderMicroservice.Models.ResponseModel;

namespace OrderMicroservice.Mediatr.Queries.ReceiveKafkaMessageQuery
{
    public class ReceiveKafkaMessageQuery<T>: IRequest<KafkaMessageStatus<T>> where T : class
    {
        public string CorrelationId { get; set; }
        public string Topic { get; set; }
        public MessageType MessageType { get; set; }
        public int Partition { get; set; }
        public int Timeout { get; set; }
    }
}