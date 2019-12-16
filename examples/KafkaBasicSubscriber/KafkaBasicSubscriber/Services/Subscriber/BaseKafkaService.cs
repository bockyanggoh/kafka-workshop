using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using KafkaBasicPublisher.OptionModel;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace KafkaBasicSubscriber.Services.Subscriber
{
    public class BaseKafkaService : BackgroundService
    {
        private readonly KafkaSubscription _subscription;
        private readonly KafkaOption _option;
        private IConsumer<Ignore, string> _consumer;
        public BaseKafkaService(KafkaOption option)
        {
            _option = option;
            Console.WriteLine(JsonConvert.SerializeObject(option));
            try
            {
                var subscription = option.Subscriptions.First(i => i.ServiceName == this.GetType().Name);
                _subscription = subscription;
                _consumer = new ConsumerBuilder<Ignore, string>(new ConsumerConfig
                        {
                            BootstrapServers = GenerateKafkaBrokerString(_option),
                            GroupId = _option.GroupId,
                            EnableAutoCommit = true,
                            StatisticsIntervalMs = 10000,
                            SessionTimeoutMs = 10000,
                            AutoOffsetReset = AutoOffsetReset.Earliest,
                            EnablePartitionEof = true
                        })
                    .SetErrorHandler((_, e) => Console.WriteLine($"Error In Kafka Consumer {e.Code}, {e.Reason}"))
                    .SetLogHandler((_, l) => Console.WriteLine($"{l.Message}"))
                    .SetPartitionsAssignedHandler((c, partitions) =>
                    {
                        Console.WriteLine($"Assigned partitions: [{string.Join(", ", partitions)}]");
                        // possibly manually specify start offsets or override the partition assignment provided by
                        // the consumer group by returning a list of topic/partition/offsets to assign to, e.g.:
                        // 
                        // return partitions.Select(tp => new TopicPartitionOffset(tp, externalOffsets[tp]));
                    })
                    .SetPartitionsRevokedHandler((c, partitions) =>
                    {
                        Console.WriteLine($"Revoking assignment: [{string.Join(", ", partitions)}]");
                    })
                    .Build();
                _consumer.Subscribe(_subscription.Topic);
                Console.WriteLine($"Kafka Service is now actively listening to Topics {_subscription.Topic}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error setting up Consumer. {e.Message}");
            }
        }
        
        private string GenerateKafkaBrokerString(KafkaOption option)
        {
            var bootstrapServers = "";
            foreach (KafkaServer k in option.Servers)
            {
                foreach (string port in k.Ports)
                {
                    bootstrapServers += string.Format("{0}:{1},", k.PublicIp, port);
                }
            }

            if (bootstrapServers.EndsWith(","))
            {
                bootstrapServers = bootstrapServers.Substring(0, bootstrapServers.Length - 1);
            }

            return bootstrapServers;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (true)
                {
                    try
                    {
                        var consumeResult = _consumer.Consume(stoppingToken);
                        if (consumeResult.IsPartitionEOF)
                        {
                            Console.WriteLine($"Reached the end of partition");
                        }
                        else
                        {
                            Console.WriteLine($"Received message, Topic {consumeResult.Topic}, Partition {consumeResult.Partition}, Offset Position {consumeResult.Offset}: {consumeResult.Value}");
                        }
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Consume error: {e.Error.Reason}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Closing consumer.");
                _consumer.Close();
            }
        }
    }
}