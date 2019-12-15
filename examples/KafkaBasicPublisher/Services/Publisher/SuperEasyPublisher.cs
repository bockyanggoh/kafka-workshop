using System;
using System.Threading.Tasks;
using KafkaBasicPublisher.OptionModel;
using KafkaBasicPublisher.RequestModel;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace KafkaBasicPublisher.Services.Publisher
{
    public class SuperEasyPublisher: BaseKafkaService
    {
        public SuperEasyPublisher(IOptions<KafkaOption> options) : base(options.Value)
        {
        }

        public async Task<bool> Publish(PublishSingleRequest request)
        {
            var response = await this.PublishToKafka(request.Message);
            return response;
        }

        public async Task PublishMultiple(PublishMultipleRequest request)
        {
            return;
        }
    }
}