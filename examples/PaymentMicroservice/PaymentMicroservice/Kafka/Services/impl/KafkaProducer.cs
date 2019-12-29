using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderMicroservice.Models.ResponseModel;
using OrderMicroservice.OptionModel;

namespace OrderMicroservice.Kafka.Services
{
    public class KafkaProducer<T> : IKafkaProducer<T> where T : class
    {
        private readonly KafkaOption _option;
        private readonly ProducerConfig _producerConfig;
        private readonly SchemaRegistryConfig _schemaRegistryConfig;
        public KafkaProducer(IOptions<KafkaOption> options)
        {
            _option = options.Value;
            _schemaRegistryConfig = new SchemaRegistryConfig
            {
                Url = $"{options.Value.Servers.SchemaRegistry.PublicIp}:{options.Value.Servers.SchemaRegistry.Port}",
                RequestTimeoutMs = 5000,
                MaxCachedSchemas = 10
            };
            _producerConfig = new ProducerConfig
            {
                BootstrapServers = GenerateKafkaBrokerString(),
                SocketTimeoutMs = 5000,
                MessageTimeoutMs = 5000,
                RequestTimeoutMs = 5000,
                Partitioner = Partitioner.Consistent
            };
        }

        public async Task<KafkaMessageStatus<T>> SendJsonMessage(string corrId, T message, string topic, string partition = "default", int timout = 5000)
        {
            if (string.IsNullOrEmpty(topic))
                throw new NullReferenceException("Topic cannot be null or empty.");

            using (var producer = new ProducerBuilder<string, string>(_producerConfig)
                .Build())
            {
                try
                {
                    var msg = JsonConvert.SerializeObject(message);
                    var res = await producer.ProduceAsync(topic, 
                        new Message<string, string>
                        {
                            Key = corrId, 
                            Value = JsonConvert.SerializeObject(message)
                        }
                    );
                    return new KafkaMessageStatus<T>
                    {
                        Success = true,
                        CorrelationId = res.Key,
                        Data = message,
                        Partition = res.Partition.Value,
                        Offset = (int)res.Offset.Value
                    };
                }
                catch (KafkaException e)
                {
                    return new KafkaMessageStatus<T>
                    {
                        Success = false,
                        ErrorInfo = $"Code:{e.Error.Code}, Reason: {e.Error.Reason}"
                    };
                }
                
            }
            
        }

        public async Task<KafkaMessageStatus<T>> SendAvroMessage(string corrId, T message, string topic, string partition = "default", int timeout = 5000)
        {
            using (var schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfig))
            using (var producer =
                new ProducerBuilder<string, T>(_producerConfig)
                    .SetKeySerializer(new AvroSerializer<string>(schemaRegistry))
                    .SetValueSerializer(new AvroSerializer<T>(schemaRegistry))
                    .SetErrorHandler((_, e) => Console.WriteLine($"Error in Kafka: {e.Reason}"))
                    .Build())
            {
                try
                {
                    Console.WriteLine($"Sending now: {DateTime.Now}");
                    var res = await producer.ProduceAsync(topic,
                        new Message<string, T> {Key = corrId, Value = message});
                    
                    Console.WriteLine($"Sent at: {DateTime.Now}");
                    return new KafkaMessageStatus<T>
                    {
                        Success = true,
                        CorrelationId = res.Key,
                        Data = message,
                        Partition = res.Partition.Value,
                        Offset = (int)res.Offset.Value
                    };    
                }
                catch (KafkaException e)
                {
                    return new KafkaMessageStatus<T>
                    {
                        Success = false,
                        ErrorInfo = $"{e.Error.Code}: {e.Error.Reason}",
                    };    
                }
            }
        }
        
        protected string GenerateKafkaBrokerString()
        {
            var bootstrapServers = "";
            foreach (KafkaBroker k in _option.Servers.Brokers)
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