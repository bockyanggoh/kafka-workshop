using Microsoft.Extensions.Options;
using OrderMicroservice.OptionModel;

namespace PaymentMicroservice.Services.Publisher
{
    public class KafkaOrderService: BaseKafkaService
    
    {
        protected KafkaOrderService(IOptions<KafkaOption> option) : base(option)
        {
        }
    }
}