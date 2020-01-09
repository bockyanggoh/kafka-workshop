using System;
using System.Threading;
using System.Threading.Tasks;
using Codegen.Avro.Models;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using KafkaConsumerAvro.OptionModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace KafkaConsumerAvro.Services.Subscriber
{
    public class SubscribeAvroSpecific : BackgroundService
    {
        private readonly KafkaOptions _option;
        private readonly ConsumerConfig _consumerConfig;
        private readonly SchemaRegistryConfig _schemaRegistryConfig;

        public SubscribeAvroSpecific(IOptions<KafkaOptions> options)
        {
            _option = options.Value;
            _consumerConfig = new ConsumerConfig
            {
                BootstrapServers = GenerateKafkaBrokerString(_option),
                GroupId = "AvroGenericListener",
                EnableAutoCommit = true,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            _schemaRegistryConfig = new SchemaRegistryConfig
            {
                Url = $"{_option.Servers.SchemaRegistry.PublicIp}:{_option.Servers.SchemaRegistry.Port}",
                RequestTimeoutMs = 5000,
                MaxCachedSchemas = 10,
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();
            using (var schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfig))
            using (var consumer =
                new ConsumerBuilder<string, MessageModel>(_consumerConfig)
                    .SetKeyDeserializer(new AvroDeserializer<string>(schemaRegistry).AsSyncOverAsync())
                    .SetValueDeserializer(new AvroDeserializer<MessageModel>(schemaRegistry).AsSyncOverAsync())
                    .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
                    .Build())
            {
                Console.WriteLine("Avro Specific Listener started.");
                consumer.Subscribe("SuperAvroSpecific");
                
                try
                {
                    while (true)
                    {
                        try
                        {
                            var consumeResult = consumer.Consume(stoppingToken);
                            Console.WriteLine($"Current offset position: {consumeResult.Offset.Value}");
                            Console.WriteLine(
                                $"Received message offset {consumeResult.Offset.Value} " +
                                $"Key: {consumeResult.Key} Partition: {consumeResult.Partition.Value}: {JsonConvert.SerializeObject(consumeResult.Message.Value)}");
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Consume error: {e.Error.Code}, {e.Error.Reason}");
                        }
                        
                        
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Forced cancel on listener");
                    consumer.Close();
                }
            }

        }


        public static string GenerateKafkaBrokerString(KafkaOptions option)
        {
            var bootstrapServers = "";
            foreach (KafkaBroker k in option.Servers.Brokers)
            {
                foreach (var port in k.Ports)
                {
                    bootstrapServers += $"{k.PublicIp}:{port},";
                }
            }

            if (bootstrapServers.EndsWith(","))
            {
                bootstrapServers = bootstrapServers.Substring(0, bootstrapServers.Length - 1);
            }

            return bootstrapServers;
        }
    }
}