using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace KafkaConsumerAvro.Services.Subscriber
{
    public class SubscribeAvroGeneric: BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("To be implemented, Generic");
            return Task.CompletedTask;
        }
    }
}