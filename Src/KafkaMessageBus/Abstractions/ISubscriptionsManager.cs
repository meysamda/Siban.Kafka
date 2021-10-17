using System;
using System.Collections.Generic;
using static KafkaMessageBus.DefaultSubscriptionsManager;

namespace KafkaMessageBus.Abstractions
{
    public interface ISubscriptionsManager
    {
        bool IsEmpty { get; }
        event EventHandler<string> OnEventRemoved;

        void AddSubscription<TMessage, TMessageHandler>();
        void AddSubscription<TMessage, TMessageHandler>(string messageName);

        void RemoveSubscription<TMessage, TMessageHandler>();
        void RemoveSubscription<TMessage, TMessageHandler>(string messageName);

        bool HasSubscriptionsForMessage<TMessage>();
        bool HasSubscriptionsForMessage(string messageName);

        Type GetMessageTypeByName(string messageName);
        void Clear();

        IEnumerable<SubscriptionInfo> GetHandlersForMessage<TMessage>();
        IEnumerable<SubscriptionInfo> GetHandlersForMessage(string messageName);

        string GetMessageKey<T>();
    }
}
