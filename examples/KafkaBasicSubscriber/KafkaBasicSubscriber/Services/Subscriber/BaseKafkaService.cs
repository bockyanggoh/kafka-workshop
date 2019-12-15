using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using KafkaBasicPublisher.OptionModel;
using KafkaBasicSubscriber.Wrappers;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace KafkaBasicSubscriber.Services.Subscriber
{
    public class BaseKafkaService : BackgroundService
    {
        private KafkaSubscription _subscription;
        private KafkaOption _option;
        public BaseKafkaService(KafkaOption option)
        {
            _option = option;
            Console.WriteLine(JsonConvert.SerializeObject(option));
            try
            {
                var subscription = option.Subscriptions.First(i => i.ServiceName == this.GetType().Name);
                _subscription = subscription;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error setting up Consumer. {e.Message}");
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

        protected void ReadErrors(Error errorMessage)
        {
            Console.WriteLine($"Error reading message, reason: {errorMessage.Reason}");
        }

        protected void ReadLogs(LogMessage m)
        {
            Console.WriteLine($"LOG MESSAGE: {m.Message}");
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var subWrapper = new SubscribeWrapper(
                        new ConsumerConfig
                        {
                            BootstrapServers = GenerateKafkaBrokerString(_option),
                            GroupId = _option.GroupId,
                            EnableAutoCommit = true,
                            StatisticsIntervalMs = 10000,
                            SessionTimeoutMs = 10000,
                            AutoOffsetReset = AutoOffsetReset.Earliest,
                            EnablePartitionEof = true
                        },
                        _subscription.Topic
                    );
                    subWrapper.ReadMessage();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error Reading: {e.Message}");
                }
            }

            return null;
        }
    }
}