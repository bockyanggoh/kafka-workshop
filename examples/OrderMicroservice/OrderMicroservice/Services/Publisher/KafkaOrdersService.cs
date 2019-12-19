using System;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using Kafka.Communication.Models;
using KafkaPublisherAvro.OptionModel;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderMicroservice.Exceptions;
using OrderMicroservice.RequestModel;
using OrderMicroservice.ResponseModel;

namespace OrderMicroservice.Services.Publisher
{
    public class KafkaOrdersService : BaseKafkaService
    {
        
        public static readonly string DATETIME_FORMAT = "MM/dd/yyyy HH:mm:ss";

        public KafkaOrdersService(IOptions<KafkaOption> option) : base(option)
        {
        }

        public async Task<KafkaPublishStatus> CreateDeliveryRequest(OrderUserRequest request)
        {
            if (request.PreferredDeliveryDate.ToUniversalTime() > DateTime.Now)
            {
                var msg = new DeliveryMessage
                {
                    CorrelationId = Guid.NewGuid().ToString(),
                    OrderIds = request.OrderIds,
                    PreferredDeliveryDate = request.PreferredDeliveryDate.ToString(DATETIME_FORMAT),
                    Username = request.Username,
                    RequestedTs = DateTime.Now.ToString(DATETIME_FORMAT)
                };

                var res = await SendMessage(msg, msg.CorrelationId);
                Console.WriteLine($"Response from Kafka: {JsonConvert.SerializeObject(res)}");
                return res;
            }
            
            throw new InvalidDateException("Specified Delivery Date is before current time.");
        }
    }
}