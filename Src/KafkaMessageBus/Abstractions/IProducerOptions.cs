namespace KafkaMessageBus.Abstractions
{
    public interface IProducerOptions<TKey, TMessage>
    where TMessage : IMessage
    {
        string Topic { get; set; }
        IKafkaMessageBusSerializer<TMessage> ValueSerializer { get; set; }
        IKafkaMessageBusSerializer<TKey> KeySerializer { get; set; }
    }
}