using System;
using System.Collections.Generic;
using static KafkaMessageBus.DefaultSubscriptionsManager;

namespace KafkaMessageBus.Abstractions
{
    public interface ISubscriptionsManager
    {
        bool IsEmpty { get; }
        event EventHandler<string> OnEventRemoved;
        void AddSubscription<TMessage, TMessageHandler>()
           where TMessage : IMessage;

        void AddSubscription<TMessage, TMessageHandler>(string messageName)
           where TMessage : IMessage;

        void RemoveSubscription<TMessage, TMessageHandler>()
            where TMessage : IMessage;

        bool HasSubscriptionsForMessage<TMessage>() where TMessage : IMessage;
        bool HasSubscriptionsForMessage(string messageName);
        Type GetMessageTypeByName(string messageName);
        void Clear();
        IEnumerable<SubscriptionInfo> GetHandlersForMessage<TMessage>() where TMessage : IMessage;
        IEnumerable<SubscriptionInfo> GetHandlersForMessage(string eventName);
        string GetMessageKey<T>();
    }
}
