using System;
using Confluent.Kafka;
using KafkaMessageBus.Abstractions;

namespace KafkaMessageBus
{
    public class DefaultPublishOptions<TKey, TMessage> : IPublishOptions<TKey, TMessage>
    {
        public IMessageBusSerializer<TMessage> ValueSerializer { get; set; }
        public IMessageBusSerializer<TKey> KeySerializer { get; set; }
        public ProducerConfig ProducerConfig { get; set; }
        public Action<Error> ErrorHandler { get; set; }
        public Action<LogMessage> LogHandler { get; set; }
    }
}