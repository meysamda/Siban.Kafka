using System.Collections.Generic;

namespace Siban.Kafka.Abstractions
{
    public interface IMessageBus : IPublishMessageBus, ISubscriptionMessageBus
    {
        new IEnumerable<string> BootstrapServers { get; }
    }
}
