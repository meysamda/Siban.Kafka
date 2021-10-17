using System;
using Confluent.Kafka;

namespace KafkaMessageBus.Abstractions
{
    public interface ISubscribeOptions<TKey, TMessage>
    {
        IMessageBusDeserializer<TKey> KeyDeserializer { get; set; }
        IMessageBusDeserializer<TMessage> ValueDeserializer { get; set; }
        ConsumerConfig ConsumerConfig { get; set; }
        Action<Error> ErrorHandler { get; set; }
        Action<LogMessage> LogHandler { get; set; }
    }
}