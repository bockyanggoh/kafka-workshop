using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Confluent.Kafka;
using KafkaPublisherContainer.OptionModel;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace KafkaPublisherContainer.Services.Publisher
{
    public class BaseKafkaService
    {
        private readonly ProducerConfig _producerConfig;
        private readonly string _defaultBrokerString;
        public BaseKafkaService(IOptions<KafkaOption> option)
        {
            Console.WriteLine(JsonConvert.SerializeObject(option));
            try
            {
                _defaultBrokerString = GenerateKafkaBrokerString(option.Value);
                _producerConfig = new ProducerConfig
                {
                    BootstrapServers = _defaultBrokerString,
                    SocketTimeoutMs = 5000,
                    MessageTimeoutMs = 3000,
                    RequestTimeoutMs = 3000,
                };
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to find Publisher information for service class {0}", this.GetType().Name);
            }
        }
        
        private string GenerateKafkaBrokerString(KafkaOption option)
        {
            var bootstrapServers = "";
            foreach (KafkaServer k in option.Servers)
            {
                foreach (string port in k.Ports)
                {
                    bootstrapServers += string.Format("{0}:{1},", k.PublicIp, port);
                }
            }

            if (bootstrapServers.EndsWith(","))
            {
                bootstrapServers = bootstrapServers.Substring(0, bootstrapServers.Length - 1);
            }

            return bootstrapServers;
        }

        public async Task<string> PublishToKafka(string message, string topic, string broker="")
        {
            if(!string.IsNullOrEmpty(broker))
                _producerConfig.BootstrapServers = broker;
            using (var producer = new ProducerBuilder<string, string>(_producerConfig).Build())
            {
                Console.WriteLine($"Producer {producer.Name} producing on topic {topic}.");

                try
                {
                    var deliveryReport = await producer.ProduceAsync(
                        topic, new Message<string, string> {Key = "Production", Value = message});

                    if (deliveryReport.Status == PersistenceStatus.Persisted)
                    {
                        Console.WriteLine("Delivery Successful!");
                        producer.Flush();
                        return "Ok";
                    }
                }
                catch (ProduceException<string, string> e)
                {
                    Console.WriteLine($"failed to deliver message: {e.Message} [{e.Error.Code}]");
                    return (e.Error.Code == ErrorCode.Local_MsgTimedOut) ?
                        "Failed to deliver message to Kafka servers. Ensure the servers are available" : e.Message;
                }
            }

            return "Failed to deliver message to Kafka servers. Ensure the servers are available";
        }
        public async Task<string> PublishMultipleMessagesToKafka(List<string> messages, string topic, string broker="default")
        {
            if(!string.IsNullOrEmpty(broker))
                _producerConfig.BootstrapServers = broker;
            using (var producer = new ProducerBuilder<string, string>(_producerConfig)
                .SetErrorHandler((_, e) => Console.WriteLine($"Error! {e.Reason}"))
                .Build())
            {
                Console.WriteLine($"Producer {producer.Name} producing on topic {topic}.");

                try
                {
                    foreach (string m in messages)
                    {
                        await producer.ProduceAsync(topic, new Message<string, string>
                        {
                            Value = m
                        });
                    }

                    return "Ok";
                }
                catch (ProduceException<string, string> e)
                {
                    
                    Console.WriteLine($"failed to deliver message: {e.Message} [{e.Error.Code}]");
                    return (e.Error.Code == ErrorCode.Local_MsgTimedOut) ?
                        "Failed to deliver message to Kafka servers. Ensure the servers are available" : e.Message;
                }
            }
            return "Failed to deliver message to Kafka servers. Ensure the servers are available";
        }
    }
}