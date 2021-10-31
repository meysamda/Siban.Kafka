using System;
using Confluent.Kafka;

namespace KafkaMessageBus.Abstractions
{
    public interface ISubscribeOptions<TKey, TMessage>
    {
        IDeserializer<TKey> KeyDeserializer { get; set; }
        IDeserializer<TMessage> ValueDeserializer { get; set; }
        ConsumerConfig ConsumerConfig { get; set; }
        Action<Error> ErrorHandler { get; set; }
        Action<LogMessage> LogHandler { get; set; }
        string ConsumerName { get; set; }
    }
}