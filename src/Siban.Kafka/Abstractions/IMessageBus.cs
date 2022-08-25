namespace Siban.Kafka.Abstractions
{
    public interface IMessageBus : IPublishMessageBus, ISubscriptionMessageBus
    {
    }
}
