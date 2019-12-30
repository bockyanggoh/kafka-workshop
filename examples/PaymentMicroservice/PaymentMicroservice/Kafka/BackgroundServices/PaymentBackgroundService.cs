using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Kafka.Communication.Models;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderMicroservice.OptionModel;
using PaymentMicroservice.Mediatr.Commands.CreatePaymentCommand;
using PaymentMicroservice.Services.Publisher;

namespace PaymentMicroservice.Kafka.BackgroundServices
{
    public class PaymentBackgroundService : IHostedService
    {
        private readonly ConsumerConfig _consumerConfig;
        private readonly KafkaSubscription _subscriptionInfo;
        private readonly SchemaRegistryConfig _schemaRegistryConfig;
        private readonly PublishPaymentResponseService _responseService;
        private readonly IMediator _mediator;
        
        public PaymentBackgroundService(IOptions<KafkaOption> options, IMediator mediator, PublishPaymentResponseService responseService)
        {
            _mediator = mediator;
            _responseService = responseService;
            var subscription = options.Value.Subscriptions.First(i => i.ServiceName == this.GetType().Name);
            if (subscription != null)
            {
                _subscriptionInfo = subscription;
            }
            _consumerConfig = new ConsumerConfig
            {
                BootstrapServers = GenerateKafkaBrokerString(options.Value),
                GroupId = "Order-MS",
                EnableAutoCommit = true,
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };

            _schemaRegistryConfig = new SchemaRegistryConfig
            {
                Url = $"{options.Value.Servers.SchemaRegistry.PublicIp}:{options.Value.Servers.SchemaRegistry.Port}",
                RequestTimeoutMs = 5000,
                MaxCachedSchemas = 10
            };
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
        private async void ListenBackground(CancellationToken cancellationToken)
        {
            using(var schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfig))
            using (var consumer = new ConsumerBuilder<string, CreatePaymentRequest>(_consumerConfig)
                .SetKeyDeserializer(new AvroDeserializer<string>(schemaRegistry).AsSyncOverAsync())
                .SetValueDeserializer(new AvroDeserializer<CreatePaymentRequest>(schemaRegistry).AsSyncOverAsync())
                .Build())
            {
                Console.WriteLine($"Starting to consume messages from {_subscriptionInfo.Topic}");
                consumer.Subscribe(_subscriptionInfo.Topic);
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(TimeSpan.FromSeconds(1));
                        if (consumeResult != null)
                        {
                            Console.WriteLine($"Received at: {DateTime.Now}");
                            var res = await _mediator.Send(new CreatePaymentCommand
                            {
                                Request = consumeResult.Value
                            });
                            Console.WriteLine($"Created payment entry: {JsonConvert.SerializeObject(res)}");
                        }
                    }
                    catch (KafkaException e)
                    {
                        Console.WriteLine($"Error consuming message: {e.Error.Reason}");
                        Console.WriteLine($"Error consuming message: {e.StackTrace}");
                    }
                }
            }
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            ListenBackground(cancellationToken);
            Console.WriteLine($"Kafka Background listener {this.GetType().Name} started.");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}