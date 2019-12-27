using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderMicroservice.Models.ResponseModel;
using OrderMicroservice.OptionModel;

namespace OrderMicroservice.Kafka.Services
{
    public class KafkaSubscriber<T>: IKafkaSubscriber<T> where T : class
    {
        private readonly ConsumerConfig _consumerConfig;
        private readonly SchemaRegistryConfig _schemaRegistryConfig;
        private readonly KafkaOption _option;

        public KafkaSubscriber(IOptions<KafkaOption> options)
        {
            _option = options.Value;
            _consumerConfig = new ConsumerConfig
            {
                BootstrapServers = GenerateKafkaBrokerString(options.Value),
                GroupId = "OrderMS",
                EnableAutoCommit = false,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                SessionTimeoutMs = 5000,
            };
            _schemaRegistryConfig = new SchemaRegistryConfig
            {
                Url = $"{options.Value.Servers.SchemaRegistry.PublicIp}:{options.Value.Servers.SchemaRegistry.Port}",
                RequestTimeoutMs = 5000,
                MaxCachedSchemas = 10
            };
            
        }


        public Task<KafkaMessageStatus<T>> ReadJsonMessage(string corrId, string topic, int timeout = 5000)
        {
            _consumerConfig.SessionTimeoutMs = timeout;
            var consumeResult = Task.Run(() =>
            {
                using (var consumer = new ConsumerBuilder<string, string>(_consumerConfig)
                    .Build())
                {
                    consumer.Subscribe(topic);
                    try
                    {
                        while (true)
                        {
                            var res = consumer.Consume();
                            if (res != null)
                            {
                                Console.WriteLine($"Received at: {DateTime.Now}");
                                if (res.Key == corrId)
                                {
                                    this.CommitAsync(consumer, res);
                                    consumer.Unsubscribe();
                                    Console.WriteLine($"Response from Payment: {res.Value}");
                                    return new KafkaMessageStatus<T>
                                    {
                                        CorrelationId = corrId,
                                        Success = true,
                                        Data = JsonConvert.DeserializeObject<T>(res.Value),
                                        Partition = res.Partition.Value,
                                        Offset = (int) res.Offset.Value
                                    };
                                }
                            }
                        }
                    }
                    catch (KafkaException e)
                    {
                        consumer.Unsubscribe();
                        return new KafkaMessageStatus<T>
                        {
                            CorrelationId = corrId,
                            Success = false,
                            ErrorInfo = $"{e.Error.Code}: {e.Error.Reason}"
                        };

                    }
                }
            });

            return consumeResult;
        }

        public Task<KafkaMessageStatus<T>> ReadAvroMessage(string corrId, string topic, int timeout = 5000)
        {
            _consumerConfig.SessionTimeoutMs = timeout;
            var consumeResult = Task.Run(() =>
            {
                using (var schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfig))
                using (var consumer = new ConsumerBuilder<string, T>(_consumerConfig)
                    .SetKeyDeserializer(new AvroDeserializer<string>(schemaRegistry).AsSyncOverAsync())
                    .SetValueDeserializer(new AvroDeserializer<T>(schemaRegistry).AsSyncOverAsync())
                    .Build())
                {
                    consumer.Subscribe(topic);
                    try
                    {
                        while (true)
                        {
                            var res = consumer.Consume();
                            if (res != null)
                            {
                                Console.WriteLine($"Received at: {DateTime.Now}");
                                if (res.Key == corrId)
                                {
                                    this.CommitAsync(consumer, res);
                                    consumer.Unsubscribe();
                                    Console.WriteLine($"Response from Payment: {res.Value}");
                                    return new KafkaMessageStatus<T>
                                    {
                                        CorrelationId = corrId,
                                        Success = true,
                                        Data = res.Value,
                                        Partition = res.Partition.Value,
                                        Offset = (int) res.Offset.Value
                                    };
                                }
                            }
                        }
                    }
                    catch (KafkaException e)
                    {
                        consumer.Unsubscribe();
                        return new KafkaMessageStatus<T>
                        {
                            CorrelationId = corrId,
                            Success = false,
                            ErrorInfo = $"{e.Error.Code}: {e.Error.Reason}"
                        };

                    }
                }
            });

            return consumeResult;
        }
        
        
        private string GenerateKafkaBrokerString(KafkaOption option)
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
        
        private void CommitAsync<T>(IConsumer<string, T> consumer, ConsumeResult<string, T> consumeResult) where T : class
        {
            consumer.Commit(consumeResult);
        }
    }
}