using Confluent.Kafka;

namespace KafkaMessageBus.Abstractions
{
    public interface IConsumerOptions<TKey, TMessage>
    where TMessage : IMessage
    {
        string GroupId { get; set; }
        IMessageBusDeserializer<TKey> KeyDeserializer { get; set; }
        IMessageBusDeserializer<TMessage> ValueDeserializer { get; set; }
        bool EnableAutoCommit { get; set; }
        AutoOffsetReset AutoOffsetReset { get; set; }
    }
}