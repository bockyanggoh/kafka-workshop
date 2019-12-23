using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using Kafka.Communication.Models;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Extensions;
using Newtonsoft.Json;
using OrderMicroservice.Domain.AggregateModel;
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

        public async Task<KafkaPublishStatus> CreatePaymentRequest(OrderEntity order, List<ItemEntity> items)
        {
            var itemCosts = new List<ItemCost>();

            foreach (var i in items)
            {
                itemCosts.Add(new ItemCost
                {
                    ItemName = i.ItemName,
                    ItemId = i.ItemId,
                    CostPrice = i.CostPrice
                });
            }

            var msg = new CreatePaymentRequest
            {
                CorrelationId = order.OrderId,
                OrderId = order.OrderId,
                PaymentStatus = order.PaymentStatus.GetDisplayName(),
                RequestedTs = order.CreatedTs.ToString(DATETIME_FORMAT),
                RequestType = "Create",
                Username = order.Username,
                CostBreakdown = itemCosts
            };

            var res = await SendMessage(msg, msg.CorrelationId);
            Console.WriteLine($"Response from Kafka: {JsonConvert.SerializeObject(res)}");
            return res;
        }
    }
}