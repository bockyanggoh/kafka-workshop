using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using KafkaBasicPublisher.OptionModel;
using Newtonsoft.Json;

namespace KafkaBasicPublisher.Services.Publisher
{
    public class BaseKafkaService
    {
        private KafkaPublisher _publisher;
        private ProducerConfig _producerConfig;
        public BaseKafkaService(KafkaOptions option)
        {
            Console.WriteLine(JsonConvert.SerializeObject(option));
            try
            {
                var publisher = option.Publishers.First(i => i.ServiceName == this.GetType().Name);
                _publisher = publisher;
                
                _producerConfig = new ProducerConfig
                {
                    BootstrapServers = GenerateKafkaBrokerString(option),
                    RequestTimeoutMs = 5000,
                };
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to find Publisher information for service class {0}", this.GetType().Name);
            }
        }
        
        protected internal static string GenerateKafkaBrokerString(KafkaOptions options)
        {
            var bootstrapServers = "";
            foreach (KafkaBroker k in options.Servers.Brokers)
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

        public async Task<bool> PublishToKafka(string message)
        {
            using (var producer = new ProducerBuilder<string, string>(_producerConfig).Build())
            {
                Console.WriteLine($"Producer {producer.Name} producing on topic {_publisher.Topic}.");

                try
                {
                    var deliveryReport = await producer.ProduceAsync(
                        _publisher.Topic, new Message<string, string> {Key = "Production", Value = message});

                    if (deliveryReport.Status == PersistenceStatus.Persisted)
                    {
                        Console.WriteLine("Delivery Successful!");
                        producer.Flush();
                        return true;
                    }
                }
                catch (ProduceException<string, string> e)
                {
                    Console.WriteLine($"failed to deliver message: {e.Message} [{e.Error.Code}]");
                }
            }
            return false;
        }

        public async Task<bool> PublishMultipleMessagesToKafka(List<string> messages)
        {
            using (var producer = new ProducerBuilder<string, string>(_producerConfig).Build())
            {
                Console.WriteLine($"Producer {producer.Name} producing on topic {_publisher.Topic}.");

                try
                {
                    foreach (string m in messages)
                    {
                        await producer.ProduceAsync(_publisher.Topic, new Message<string, string>
                        {
                            Value = m
                        });
                    }

                    return true;
                }
                catch (ProduceException<string, string> e)
                {
                    Console.WriteLine($"failed to deliver message: {e.Message} [{e.Error.Code}]");
                }
            }
            return false;
        }
    }
}