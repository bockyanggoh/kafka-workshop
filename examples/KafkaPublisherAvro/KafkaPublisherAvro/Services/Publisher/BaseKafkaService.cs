using System;
using System.Threading;
using System.Threading.Tasks;
using Avro;
using Avro.Generic;
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
        public BaseKafkaService(IOptions<KafkaOption> option)
        {
            Console.WriteLine(JsonConvert.SerializeObject(option));
            try
            {
                _defaultBrokerString = GenerateKafkaBrokerString(option.Value);
                _schemaRegistryUrl =
                    $"{option.Value.Servers.SchemaRegistry.PublicIp}:{option.Value.Servers.SchemaRegistry.Port}";
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to find Publisher information for service class {0}", this.GetType().Name);
            }
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

        public async Task<string> PublishAvroMessage(PublishSingleRequest request)
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
    }
}