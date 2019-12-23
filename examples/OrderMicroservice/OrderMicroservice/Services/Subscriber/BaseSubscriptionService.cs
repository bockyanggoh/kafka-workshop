using System;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Options;
using OrderMicroservice.Models.ResponseModel;
using OrderMicroservice.OptionModel;

namespace OrderMicroservice.Services.Subscriber
{
    public class BaseSubscriptionService
    {
        private readonly ConsumerConfig _consumerConfig;
        private readonly KafkaSubscription _subscriptionInfo;
        private readonly SchemaRegistryConfig _schemaRegistryConfig;

        public BaseSubscriptionService(IOptions<KafkaOption> options)
        {
            var subscription = options.Value.Subscriptions.First(i => i.ServiceName == this.GetType().Name);
            if (subscription != null)
            {
                _subscriptionInfo = subscription;
            }
            _consumerConfig = new ConsumerConfig
            {
                BootstrapServers = GenerateKafkaBrokerString(options.Value),
                GroupId = "Order-MS",
                EnableAutoCommit = false,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                SessionTimeoutMs = 5000,
                SocketTimeoutMs = 5000
            };

            _schemaRegistryConfig = new SchemaRegistryConfig
            {
                Url = $"{options.Value.Servers.SchemaRegistry.PublicIp}:{options.Value.Servers.SchemaRegistry.Port}",
                RequestTimeoutMs = 5000,
                MaxCachedSchemas = 10
            };
        }

        public async Task<KafkaMessageStatus<T>> WaitForResponse<T>(string correlationId, int timeout=5000) where T : class
        {
            _consumerConfig.SessionTimeoutMs = timeout;
            _consumerConfig.SocketTimeoutMs = timeout;
            using(var schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfig))
            using (var consumer = new ConsumerBuilder<string, T>(_consumerConfig)
                .SetKeyDeserializer(new AvroDeserializer<string>(schemaRegistry).AsSyncOverAsync())
                .SetValueDeserializer(new AvroDeserializer<T>(schemaRegistry).AsSyncOverAsync())
                .Build())
            {
                consumer.Subscribe(_subscriptionInfo.Topic);
                while (true)
                {
                    try
                    {
                        var consumeResult = consumer.Consume();
                        if (consumeResult.Key == correlationId)
                        {
                            consumer.Commit(consumeResult);
                            consumer.Unsubscribe();
                            return new KafkaMessageStatus<T>
                            {
                                CorrelationId = correlationId,
                                Success = true,
                                Data = consumeResult.Value
                            };
                        }
                    }
                    catch (KafkaException e)
                    {
                        consumer.Unsubscribe();
                        return new KafkaMessageStatus<T>
                        {
                            CorrelationId = correlationId,
                            Success = false,
                            ErrorInfo = $"{e.Error.Code}: {e.Error.Reason}"
                        };
                        
                    }
                }
            }
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