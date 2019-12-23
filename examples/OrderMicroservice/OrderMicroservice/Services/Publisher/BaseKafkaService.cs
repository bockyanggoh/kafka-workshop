using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avro;
using Avro.Generic;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderMicroservice.Models.ResponseModel;
using OrderMicroservice.OptionModel;
using OrderMicroservice.RequestModel;

namespace OrderMicroservice.Services.Publisher
{
    public class BaseKafkaService
    {
        
        private KafkaPublisher _publisher;
        private ProducerConfig _producerConfig;
        private readonly string _schemaRegistryUrl;
        private readonly AvroSerializerConfig _avroSerializerConfig;
        protected BaseKafkaService(IOptions<KafkaOption> option)
        {
            Console.WriteLine(JsonConvert.SerializeObject(option));
            
            try
            {
                var publisher = option.Value.Publishers.First(i => i.ServiceName == this.GetType().Name);
                if (publisher != null)
                {
                    _publisher = publisher;
                    _schemaRegistryUrl =
                        $"{option.Value.Servers.SchemaRegistry.PublicIp}:{option.Value.Servers.SchemaRegistry.Port}";
                    _avroSerializerConfig = new AvroSerializerConfig
                    {
                        BufferBytes = 500,
                        AutoRegisterSchemas = true
                    };
                    _producerConfig = new ProducerConfig
                    {
                        BootstrapServers = GenerateKafkaBrokerString(option.Value),
                        SocketTimeoutMs = 5000,
                        MessageTimeoutMs = 5000,
                        RequestTimeoutMs = 5000
                    };
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to find Publisher information for service class {0}", this.GetType().Name);
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

        protected async Task<KafkaPublishStatus> SendMessage<T>(T request, string correlationId) where T : class
        {
            var cts = new CancellationToken();
            using (var schemaRegistry = new CachedSchemaRegistryClient(new SchemaRegistryConfig
            {
                Url = _schemaRegistryUrl,
                RequestTimeoutMs = 5000,
                MaxCachedSchemas = 10
            }))
            using (var producer =
                new ProducerBuilder<string, T>(_producerConfig)
                    .SetKeySerializer(new AvroSerializer<string>(schemaRegistry))
                    .SetValueSerializer(new AvroSerializer<T>(schemaRegistry, _avroSerializerConfig))
                    .SetErrorHandler((_, e) => Console.WriteLine($"Error in Kafka: {e.Reason}"))
                    .Build())
            {
                try
                {
                    var res = await producer.ProduceAsync(_publisher.Topic,
                        new Message<string, T> {Key = correlationId, Value = request});
                    
                    return new KafkaPublishStatus
                    {
                        Status = "Successful",
                        Success = true,
                        CorrelationId = res.Key
                    };    
                }
                catch (KafkaException e)
                {
                    return new KafkaPublishStatus
                    {
                        Status = "Failed",
                        Success = false,
                        ErrorInfo = $"{e.Error.Code}: {e.Error.Reason}",
                        CorrelationId = correlationId
                    };    
                }
            }
        }
    }
}