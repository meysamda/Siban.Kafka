using System.Collections.Generic;
using Confluent.Kafka;
using KafkaMessageBus.Abstractions;

namespace KafkaMessageBus
{
    public class DefaultConsumerOptions<TKey, TMessage> : IConsumerOptions<TKey, TMessage>
    where TMessage : IMessage
    {
        public string GroupId { get; set; }
        public IEnumerable<string> Topics { get; set; }
        public TKey Key { get; set; }
        public int TimeOutMilliseconds { get; set; }
        public IMessageBusDeserializer<TKey> KeyDeserializer { get; set; }
        public IMessageBusDeserializer<TMessage> ValueDeserializer { get; set; }
        public bool EnableAutoCommit { get; set; }
        public AutoOffsetReset AutoOffsetReset { get; set; }
        public bool AllowAutoCreateTopics { get; set; }
    }
}