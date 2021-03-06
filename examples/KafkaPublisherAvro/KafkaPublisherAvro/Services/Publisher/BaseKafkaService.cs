using System;
using System.Threading;
using System.Threading.Tasks;
using Avro;
using Avro.Generic;
using Codegen.Avro.Models;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using KafkaPublisherAvro.OptionModel;
using KafkaPublisherAvro.RequestModel;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace KafkaPublisherAvro.Services.Publisher
{
    public class BaseKafkaService
    {
        private readonly string _defaultBrokerString;
        private readonly string _schemaRegistryUrl;
        private readonly AvroSerializerConfig _avroSerializerConfig;
        public BaseKafkaService(IOptions<KafkaOptions> option)
        {
            Console.WriteLine(JsonConvert.SerializeObject(option));
            
            try
            {
                _defaultBrokerString = GenerateKafkaBrokerString(option.Value);
                _schemaRegistryUrl =
                    $"{option.Value.Servers.SchemaRegistry.PublicIp}:{option.Value.Servers.SchemaRegistry.Port}";
                _avroSerializerConfig = new AvroSerializerConfig
                {
                    BufferBytes = 500,
                    AutoRegisterSchemas = true
                };
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to find Publisher information for service class {0}", this.GetType().Name);
            }
        }
        
        private string GenerateKafkaBrokerString(KafkaOptions option)
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

        public async Task<string> PublishGenericAvroMessage(PublishSingleRequest request)
        {
            var schema = (RecordSchema)RecordSchema.Parse(
                @"{
                    ""namespace"": ""Kafka.Workshop.Avro"",
                    ""type"": ""record"",
                    ""name"": ""JustAMessage"",
                    ""fields"": [
                        {""name"": ""title"", ""type"": ""string""},
                        {""name"": ""message"",  ""type"": ""string""}
                    ]
                  }"
            );
            
            var cts = new CancellationTokenSource();
            using (var schemaRegistry = new CachedSchemaRegistryClient(new SchemaRegistryConfig
            {
                Url = _schemaRegistryUrl
            }))
            using (var producer =
                new ProducerBuilder<string, GenericRecord>(new ProducerConfig
                    {
                        BootstrapServers = _defaultBrokerString,
                        SocketTimeoutMs = 5000,
                        MessageTimeoutMs = 3000,
                        RequestTimeoutMs = 3000,
                    })
                    .SetKeySerializer(new AvroSerializer<string>(schemaRegistry))
                    .SetValueSerializer(new AvroSerializer<GenericRecord>(schemaRegistry))
                    .Build())
            {
                var record = new GenericRecord(schema);
                record.Add("title", request.Message.Title);
                record.Add("message", request.Message.Message);
                var res = await producer
                    .ProduceAsync(request.Topic, new Message<string, GenericRecord> {Key = "producer", Value = record})
                    .ContinueWith(task => task.IsFaulted
                        ? $"Error producing message {task.Exception.Message}"
                        : $"Message sent to: {task.Result.TopicPartitionOffset}", cts.Token);
                return res;
            }
        }

        public async Task<string> PublishSpecificAvroMessage(PublishSingleRequest request)
        {
            
            var cts = new CancellationToken();
            using (var schemaRegistry = new CachedSchemaRegistryClient(new SchemaRegistryConfig
            {
                Url = _schemaRegistryUrl,
                RequestTimeoutMs = 5000,
                MaxCachedSchemas = 10
            }))
            using (var producer =
                new ProducerBuilder<string, MessageModel>(new ProducerConfig
                    {
                        BootstrapServers = _defaultBrokerString,
                        SocketTimeoutMs = 5000,
                        MessageTimeoutMs = 3000,
                        RequestTimeoutMs = 3000,
                    })
                    .SetKeySerializer(new AvroSerializer<string>(schemaRegistry))
                    .SetValueSerializer(new AvroSerializer<MessageModel>(schemaRegistry))
                    .SetErrorHandler((_, e) => Console.WriteLine($"Error in Kafka: {e.Reason}"))
                    .Build())
            {
                var msg = new MessageModel
                {
                    Title = request.Message.Title,
                    Message = request.Message.Message,
                    CorrelationId = Guid.NewGuid().ToString(),
                    TransactionTs = DateTime.Now.ToString()
                };
                Console.WriteLine($"Pushing message to kafka. {JsonConvert.SerializeObject(msg)}");
                var res = await producer.ProduceAsync(request.Topic, new Message<string, MessageModel>{Key = "Producer", Value = msg})
                    .ContinueWith(task => task.IsFaulted
                        ? $"error producing message: {task.Exception.Message}"
                        : $"produced to: {task.Result.TopicPartitionOffset}", cts);

                return res;
            }
        }
    }
}