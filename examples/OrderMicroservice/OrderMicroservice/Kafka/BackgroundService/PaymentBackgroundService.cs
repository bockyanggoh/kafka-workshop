using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Kafka.Communication.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderMicroservice.OptionModel;

namespace OrderMicroservice.Kafka.BackgroundService
{
    public class PaymentBackgroundService : IHostedService
    {
        private readonly SchemaRegistryConfig _schemaRegistryConfig;
        private readonly ConsumerConfig _consumerConfig;

        public PaymentBackgroundService(IOptions<KafkaOption> options,SchemaRegistryConfig schemaRegistryConfig)
        {
            _schemaRegistryConfig = schemaRegistryConfig;
            _consumerConfig = new ConsumerConfig
            {
                BootstrapServers = GenerateKafkaBrokerString(options.Value),
                GroupId = "Order-MS",
                EnableAutoCommit = true,
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };
            _schemaRegistryConfig = new SchemaRegistryConfig
            {
                Url = $"{options.Value.Servers.SchemaRegistry.PublicIp}:{options.Value.Servers.SchemaRegistry.Port}",
                RequestTimeoutMs = 5000,
                MaxCachedSchemas = 10
            };
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            using(var schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfig))
            using (var consumer = new ConsumerBuilder<string, CreatePaymentResponse>(_consumerConfig)
                .SetKeyDeserializer(new AvroDeserializer<string>(schemaRegistry).AsSyncOverAsync())
                .SetValueDeserializer(new AvroDeserializer<CreatePaymentResponse>(schemaRegistry).AsSyncOverAsync())
                .Build())
            {
                consumer.Subscribe("PaymentResponseAvro");
                while (!cancellationToken.IsCancellationRequested)
                {
                    var res = consumer.Consume(TimeSpan.FromMilliseconds(500));
                    if (res != null)
                    {
                        
                        Console.WriteLine($"Message {DateTime.Now} Key: {res.Key}, Value: {JsonConvert.SerializeObject(res.Value)}");
                    }
                }
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
        
        protected string GenerateKafkaBrokerString(KafkaOption option)
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