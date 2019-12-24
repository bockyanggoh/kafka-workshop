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
using PaymentMicroservice.Domain.AggregateModel;
using PaymentMicroservice.Mediatr.Commands.CreatePaymentCommand;
using PaymentMicroservice.Services.Publisher;

namespace PaymentMicroservice.Services.Subscriber
{
    public class SubscribePaymentService: BackgroundService
    {
        private readonly ConsumerConfig _consumerConfig;
        private readonly KafkaSubscription _subscriptionInfo;
        private readonly SchemaRegistryConfig _schemaRegistryConfig;
        private readonly PublishPaymentResponseService _responseService;
        private readonly IMediator _mediator;
        
        public SubscribePaymentService(IOptions<KafkaOption> options, IMediator mediator, PublishPaymentResponseService responseService)
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
        
        protected override async  Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using(var schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfig))
            using (var consumer = new ConsumerBuilder<string, CreatePaymentRequest>(_consumerConfig)
                .SetKeyDeserializer(new AvroDeserializer<string>(schemaRegistry).AsSyncOverAsync())
                .SetValueDeserializer(new AvroDeserializer<CreatePaymentRequest>(schemaRegistry).AsSyncOverAsync())
                .Build())
            {
                consumer.Subscribe(_subscriptionInfo.Topic);
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = consumer.Consume();
                        var res = await _mediator.Send(new CreatePaymentCommand
                        {
                            Request = consumeResult.Value
                        });
                        Console.WriteLine($"Created payment entry: {JsonConvert.SerializeObject(res)}");
                        try
                        {
                            await _responseService.SendPaymentResponse(res, consumeResult.Key);
                        }
                        catch (KafkaException e)
                        {
                            //Error, TODO: Rollback payment if response sending fails.
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

    }
}