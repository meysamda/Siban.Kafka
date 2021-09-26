using System.Collections.Generic;
using Confluent.Kafka;

namespace KafkaMessageBus.Abstractions
{
    public interface IConsumerOptions<TKey, TMessage>
    where TMessage : IMessage
    {
        IEnumerable<string> Topics { get; set; }
        string GroupId { get; set; }
        IKafkaMessageBusDeserializer<TKey> KeyDeserializer { get; set; }
        IKafkaMessageBusDeserializer<TMessage> ValueDeserializer { get; set; }
        bool EnableAutoCommit { get; set; }
        AutoOffsetReset AutoOffsetReset { get; set; }
    }
}