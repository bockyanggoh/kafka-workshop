using System;
using System.Linq;
using System.Threading;
using Confluent.Kafka;
using KafkaBasicPublisher.OptionModel;
using Newtonsoft.Json;

namespace KafkaBasicSubscriber.Services.Subscriber
{
    public class BaseKafkaService
    {
        private KafkaSubscription _subscription;
        private ConsumerConfig _consumerConfig;
        public BaseKafkaService(KafkaOption option)
        {
            Console.WriteLine(JsonConvert.SerializeObject(option));
            try
            {
                var subscription = option.Subscriptions.First(i => i.ServiceName == this.GetType().Name);
                _subscription = subscription;
                
                _consumerConfig = new ConsumerConfig
                {
                    BootstrapServers = GenerateKafkaBrokerString(option),
                    GroupId = option.GroupId,
                    EnableAutoCommit = false,
                    StatisticsIntervalMs = 10000,
                    SessionTimeoutMs = 10000,
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    EnablePartitionEof = true
                };
                
                this.Consume();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error consuming. {e.Message}");
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

        protected void Consume()
        {
            using (var consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig)
                .SetErrorHandler((_, e) => ReadErrors(e))
                .SetLogHandler((_, e) => ReadLogs(e))
                .SetPartitionsAssignedHandler((c, partitions) =>
                {
                    Console.WriteLine($"Assigned partitions: [{string.Join(", ", partitions)}]");
                })
                .SetPartitionsRevokedHandler((c, partitions) =>
                {
                    Console.WriteLine($"Revoking assignment: [{string.Join(", ", partitions)}]");
                })
                .Build()
            )
            {
                consumer.Subscribe(_subscription.Topic);

                ReadMessage(consumer);
            }
        }

        protected void ReadErrors(Error errorMessage)
        {
            Console.WriteLine($"Error reading message, reason: {errorMessage.Reason}");
        }

        protected void ReadLogs(LogMessage m)
        {
            Console.WriteLine($"LOG MESSAGE: {m.Message}");
        }

        protected void ReadMessage(IConsumer<Ignore, string> consumer)
        {
            try
            {
                while (true)
                {
                    var consumeResult = consumer.Consume(CancellationToken.None);
                    if (consumeResult.IsPartitionEOF)
                    {
                        Console.WriteLine(
                            $"Reached end of topic {consumeResult.Topic}, partition {consumeResult.Partition}, offset {consumeResult.Offset}");
                    }

                    Console.WriteLine(
                        $"Received message at {consumeResult.TopicPartitionOffset}: {consumeResult.Value}");

                    if (consumeResult.Offset % 1 == 0)
                    {
                        try
                        {
                            consumer.Commit(consumeResult);
                        }
                        catch (KafkaException e)
                        {
                            Console.WriteLine($"Commit error: ${e.Error.Reason}");
                        }
                    }
                }
            }
            catch (ConsumeException e)
            {
                Console.WriteLine($"Consume error: {e.Error.Reason}");
            }
        }

    }
}