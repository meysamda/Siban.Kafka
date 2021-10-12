using System;
using Confluent.Kafka;
using KafkaMessageBus.Abstractions;

namespace KafkaMessageBus
{
    public class DefaultProducerOptions<TKey, TMessage> : IProducerOptions<TKey, TMessage>
    where TMessage : IMessage
    {
        public IMessageBusSerializer<TMessage> ValueSerializer { get; set; }
        public IMessageBusSerializer<TKey> KeySerializer { get; set; }
    }
}