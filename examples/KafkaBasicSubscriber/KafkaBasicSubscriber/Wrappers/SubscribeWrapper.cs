using Confluent.Kafka;
using System;
using System.Threading;

namespace KafkaBasicSubscriber.Wrappers
{
    public class SubscribeWrapper
    {
        private readonly IConsumer<Ignore, string> _consumer;

        public SubscribeWrapper(ConsumerConfig consumerConfig, string topic)
        {
            _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig)
                .Build();
            _consumer.Subscribe(topic);
        }

        public string ReadMessage()
        {
            var consumeResult = this._consumer.Consume();
            return consumeResult.Value;
        }
    }
}