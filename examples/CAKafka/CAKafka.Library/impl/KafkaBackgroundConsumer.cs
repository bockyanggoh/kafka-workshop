using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CAKafka.Domain.Models;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CAKafka.Library.impl
{
    public class KafkaBackgroundConsumer : BackgroundService
    {
        private readonly ILogger<KafkaBackgroundConsumer> _logger;
        private readonly KafkaOptions _options;
        private readonly ConsumerConfig _consumerConfig;
        private readonly List<string> _topics;
    
        public KafkaBackgroundConsumer(ILogger<KafkaBackgroundConsumer> logger, KafkaOptions options, List<string> topics)
        {
            _logger = logger;
            _options = options;
            _consumerConfig = new ConsumerConfig
            {
                BootstrapServers = KafkaMethods.GenerateKafkaBrokerString(options),
                GroupId = _options.GroupId,
                EnableAutoCommit = true,
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };
            _topics = topics;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var consumer = new ConsumerBuilder<string, string>(_consumerConfig)
                .Build())
            {
                consumer.Subscribe(_topics);
                _logger.LogInformation($"Started Kafka Listener on Topics: {JsonConvert.SerializeObject(_topics)}");
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(TimeSpan.FromMilliseconds(_options.SubscriptionPollIntervalMs));
                        if (consumeResult != null)
                        {
                            ProcessingLogic(consumeResult);
                        }
                    }
                    catch (KafkaException e)
                    {
                        _logger.LogError($"Error consuming message from Kafka. Code: {e.Error.Code}, Message: {e.Error.Reason}");
                    }
                }
            }

            return Task.CompletedTask;
        }

        public virtual Task ProcessingLogic(ConsumeResult<string, string> message)
        {
            _logger.LogWarning("This is a generic placeholder for processing logic, you should really override ProcessingLogic method to implement your business functions.");
            _logger.LogInformation($"Nevertheless, Message received. Topic: {message.Topic}, Partition: {message.Partition.Value}");
            return Task.CompletedTask;
        }
    }
}