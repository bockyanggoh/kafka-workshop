using KafkaBasicPublisher.OptionModel;
using Microsoft.Extensions.Options;

namespace KafkaBasicSubscriber.Services.Subscriber
{
    public class SuperEasySubscriber: BaseKafkaService
    {
        public SuperEasySubscriber(IOptions<KafkaOption> options) : base(options.Value)
        {
            
        }
    }
}