using System.Threading;
using System.Threading.Tasks;
using KafkaPublisherAvro.OptionModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace KafkaConsumerAvro.Services.Subscriber
{
    public class BaseSubscriberService : BackgroundService
    {
        public BaseSubscriberService(IOptions<KafkaOption> options)
        {
            
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new System.NotImplementedException();
        }
    }
}