using System;
using Confluent.Kafka;

namespace KafkaMessageBus.Abstractions
{
    public interface IPublishOptions<TKey, TMessage>
    {
        ISerializer<TMessage> ValueSerializer { get; set; }
        ISerializer<TKey> KeySerializer { get; set; }
        ProducerConfig ProducerConfig { get; set; }
        Action<Error> ErrorHandler { get; set; }
        Action<LogMessage> LogHandler { get; set; }
        string ProducerName { get; set; }
    }
}