using System;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using Kafka.Communication.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderMicroservice.Exceptions;
using OrderMicroservice.Models.ResponseModel;
using OrderMicroservice.OptionModel;
using OrderMicroservice.RequestModel;

namespace OrderMicroservice.Services.Publisher
{
    public class KafkaOrdersService : BaseKafkaService
    {
        
        public static readonly string DATETIME_FORMAT = "MM/dd/yyyy HH:mm:ss";

        public KafkaOrdersService(IOptions<KafkaOption> option) : base(option)
        {
        }

        public async Task<KafkaPublishStatus> CreatePaymentRequest(OrderUserRequest request)
        {
            var msg = new DeliveryMessage
            {
                CorrelationId = Guid.NewGuid().ToString(),
                OrderIds = request.OrderIds,
                Username = request.Username,
                RequestedTs = DateTime.Now.ToString(DATETIME_FORMAT)
            };

            var res = await SendMessage(msg, msg.CorrelationId);
            Console.WriteLine($"Response from Kafka: {JsonConvert.SerializeObject(res)}");
            return res;
        }
    }
}