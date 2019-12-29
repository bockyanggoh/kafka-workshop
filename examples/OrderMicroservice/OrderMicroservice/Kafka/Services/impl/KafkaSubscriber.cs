using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderMicroservice.Kafka.Models;
using OrderMicroservice.OptionModel;

namespace OrderMicroservice.Kafka.Services.impl
{
    public class KafkaSubscriber<T>: IKafkaSubscriber<T> where T : class
    {
        private readonly ConsumerConfig _consumerConfig;
        private readonly SchemaRegistryConfig _schemaRegistryConfig;
        private readonly KafkaOption _option;
        private readonly IConsumer<string, T> _avroConsumer;

        public KafkaSubscriber(IOptions<KafkaOption> options)
        {
            _option = options.Value;
            _consumerConfig = new ConsumerConfig
            {
                BootstrapServers = GenerateKafkaBrokerString(options.Value),
                GroupId = "OrderMS",
                EnableAutoCommit = false,
                AutoOffsetReset = AutoOffsetReset.Earliest,
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
            var consumeResult = Task.Run(() =>
            {
                Console.WriteLine($"Starting to consume Json messages from {topic} with CorrId {corrId}");
                using (var consumer = new ConsumerBuilder<string, string>(_consumerConfig)
                    .Build())
                {
                    consumer.Subscribe(topic);
                    try
                    {
                        var splitDelay = timeout / 100;
                        for (int i = 0; i < 100; i++)
                        {
                            var res = consumer.Consume(TimeSpan.FromMilliseconds(splitDelay));
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
                        return new KafkaMessageStatus<T>
                        {
                            CorrelationId = corrId,
                            Success = false,
                            ErrorInfo = $"Payment failed to respond to CorrelationId {corrId}. Request is nulled."
                        };
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
            
            var consumeResult = Task.Run(() =>
            {
                Console.WriteLine($"Starting to consume Avro messages from {topic} with CorrId {corrId}");

                using (var schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfig))
                using (var consumer = new ConsumerBuilder<string, T>(_consumerConfig)
                    .SetKeyDeserializer(new AvroDeserializer<string>(schemaRegistry).AsSyncOverAsync())
                    .SetValueDeserializer(new AvroDeserializer<T>(schemaRegistry).AsSyncOverAsync())
                    .Build())
                {
                    Console.WriteLine($"Logged at: {DateTime.Now}");
                    consumer.Subscribe(topic);
                    
                    Console.WriteLine($"Logged at: {DateTime.Now}");
                    try
                    {
                        var shouldIquit = false;
                        while (!shouldIquit)
                        {
                            var res = consumer.Consume(TimeSpan.FromMilliseconds(100));
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
                        return new KafkaMessageStatus<T>
                        {
                            CorrelationId = corrId,
                            Success = false,
                            ErrorInfo = "No response from Payment Microservice."
                        };
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