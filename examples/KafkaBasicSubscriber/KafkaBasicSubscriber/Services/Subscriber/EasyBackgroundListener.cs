using System.Collections.Generic;
using CAKafka.Domain.Models;
using CAKafka.Library.impl;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KafkaBasicSubscriber.Services.Subscriber
{
    public class EasyBackgroundListener : KafkaBackgroundConsumer
    {
        public EasyBackgroundListener(ILogger<KafkaBackgroundConsumer> logger, IOptions<KafkaOptions> options) : base(logger, options.Value, new List<string>{"SuperEasy"})
        {
        }
    }
}