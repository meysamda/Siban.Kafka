using System;
using Confluent.Kafka;

namespace KafkaMessageBus.Abstractions
{
    public interface IPublishOptions<TKey, TMessage>
    where TMessage : IMessage
    {
        IMessageBusSerializer<TMessage> ValueSerializer { get; set; }
        IMessageBusSerializer<TKey> KeySerializer { get; set; }
        ProducerConfig ProducerConfig { get; set; }
        Action<Error> ErrorHandler { get; set; }
        Action<LogMessage> LogHandler { get; set; }
    }
}