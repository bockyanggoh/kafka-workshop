using System.Threading.Tasks;
using Kafka.Communication.Models;
using Microsoft.Extensions.Options;
using OrderMicroservice.Models.ResponseModel;
using OrderMicroservice.OptionModel;

namespace OrderMicroservice.Services.Subscriber
{
    public class SubscribeOrderService : BaseSubscriptionService
    {
        public SubscribeOrderService(IOptions<KafkaOption> options) : base(options)
        {
        }

        public async Task<KafkaMessageStatus<CreatePaymentResponse>> WaitForMessage(string correlationId, int timeout=5000)
        {
            var res = await WaitForResponse<CreatePaymentResponse>(correlationId, timeout);
            return res;
        }
    }
}