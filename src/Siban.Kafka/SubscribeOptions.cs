using System;
using Confluent.Kafka;
using Siban.Kafka.Abstractions;

namespace Siban.Kafka
{
    public class SubscribeOptions<TKey, TMessage> : ISubscribeOptions<TKey, TMessage>
    {
        public IDeserializer<TKey> KeyDeserializer { get; set; }
        public IDeserializer<TMessage> ValueDeserializer { get; set; }
        public ConsumerConfig ConsumerConfig { get; set; }
        public Action<Error> ErrorHandler { get; set; }
        public Action<LogMessage> LogHandler { get; set; }
        public string ConsumerName { get; set; }
    }
}