using System;
using Confluent.Kafka;

namespace Siban.Kafka.Abstractions
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