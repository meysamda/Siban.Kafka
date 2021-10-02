namespace KafkaMessageBus.Abstractions
{
    public interface IProducerOptions<TKey, TMessage>
    where TMessage : IMessage
    {
        IMessageBusSerializer<TMessage> ValueSerializer { get; set; }
        IMessageBusSerializer<TKey> KeySerializer { get; set; }
    }
}