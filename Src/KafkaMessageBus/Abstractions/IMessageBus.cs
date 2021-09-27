namespace KafkaMessageBus.Abstractions
{
    public interface IMessageBus : IPublishMessageBus, ISubscriptionMessageBus
    {
    }
}
