using KafkaMessageBus.Abstractions;

namespace KafkaMessageBus
{
    public class DefaultProduceOptions<TKey, TMessage> : IProducerOptions<TKey, TMessage>
    where TMessage : IMessage
    {
        public string Topic { get; set; }
        public IKafkaMessageBusSerializer<TMessage> ValueSerializer { get; set; }
        public IKafkaMessageBusSerializer<TKey> KeySerializer { get; set; }
    }
}