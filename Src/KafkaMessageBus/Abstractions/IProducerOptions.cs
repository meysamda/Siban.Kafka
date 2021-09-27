namespace KafkaMessageBus.Abstractions
{
    public interface IProducerOptions<TKey, TMessage>
    where TMessage : IMessage
    {
        string Topic { get; set; }
        IMessageBusSerializer<TMessage> ValueSerializer { get; set; }
        IMessageBusSerializer<TKey> KeySerializer { get; set; }
    }
}